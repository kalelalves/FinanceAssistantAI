create extension if not exists pgcrypto;

create table if not exists public.profiles (
    id uuid primary key references auth.users(id) on delete cascade,
    email text,
    display_name text,
    created_at timestamptz not null default now(),
    updated_at timestamptz not null default now()
);

create table if not exists public.analysis_requests (
    id uuid primary key default gen_random_uuid(),
    user_id uuid not null references auth.users(id) on delete cascade,
    status text not null default 'pending',
    assets_count integer not null default 0,
    requested_at timestamptz not null default now(),
    completed_at timestamptz,
    error_message text,
    constraint analysis_requests_status_check
        check (status in ('pending', 'processing', 'completed', 'failed')),
    constraint analysis_requests_assets_count_check
        check (assets_count between 0 and 10)
);

create table if not exists public.analysis_assets (
    id uuid primary key default gen_random_uuid(),
    analysis_request_id uuid not null references public.analysis_requests(id) on delete cascade,
    ticker text not null,
    normalized_ticker text generated always as (upper(trim(ticker))) stored,
    created_at timestamptz not null default now(),
    constraint analysis_assets_ticker_not_empty_check
        check (length(trim(ticker)) > 0),
    constraint analysis_assets_ticker_unique
        unique (analysis_request_id, normalized_ticker)
);

create table if not exists public.asset_fundamentals_snapshot (
    id uuid primary key default gen_random_uuid(),
    analysis_asset_id uuid not null references public.analysis_assets(id) on delete cascade,
    regular_market_price numeric(18, 4),
    dividend_yield numeric(18, 6),
    price_to_earnings numeric(18, 6),
    source text not null default 'brapi.dev',
    raw_payload jsonb,
    captured_at timestamptz not null default now()
);

create table if not exists public.macro_indicators_snapshot (
    id uuid primary key default gen_random_uuid(),
    analysis_request_id uuid not null references public.analysis_requests(id) on delete cascade,
    selic_meta_annual numeric(18, 6),
    ipca_monthly numeric(18, 6),
    ipca_12m numeric(18, 6),
    usd_ptax_sell numeric(18, 6),
    savings_monthly numeric(18, 6),
    source text not null default 'bcb_sgs',
    raw_payload jsonb,
    captured_at timestamptz not null default now()
);

create table if not exists public.ai_analysis_results (
    id uuid primary key default gen_random_uuid(),
    analysis_asset_id uuid not null references public.analysis_assets(id) on delete cascade,
    target_price numeric(18, 4),
    horizon text not null,
    diagnosis text not null,
    summary text not null,
    model text,
    prompt_version text,
    input_tokens integer,
    output_tokens integer,
    raw_payload jsonb,
    created_at timestamptz not null default now(),
    constraint ai_analysis_results_horizon_check
        check (horizon in ('short_term', 'medium_term', 'long_term')),
    constraint ai_analysis_results_diagnosis_check
        check (diagnosis in ('buy', 'hold', 'watch', 'avoid'))
);

create table if not exists public.usage_events (
    id uuid primary key default gen_random_uuid(),
    user_id uuid references auth.users(id) on delete set null,
    analysis_request_id uuid references public.analysis_requests(id) on delete set null,
    event_type text not null,
    provider text,
    ticker text,
    status text not null,
    input_tokens integer,
    output_tokens integer,
    latency_ms integer,
    message text,
    created_at timestamptz not null default now()
);

create index if not exists ix_analysis_requests_user_requested_at
    on public.analysis_requests(user_id, requested_at desc);

create index if not exists ix_analysis_assets_request
    on public.analysis_assets(analysis_request_id);

create index if not exists ix_usage_events_user_created_at
    on public.usage_events(user_id, created_at desc);

create or replace function public.set_updated_at()
returns trigger
language plpgsql
as $$
begin
    new.updated_at = now();
    return new;
end;
$$;

drop trigger if exists profiles_set_updated_at on public.profiles;
create trigger profiles_set_updated_at
before update on public.profiles
for each row execute function public.set_updated_at();

create or replace function public.refresh_analysis_assets_count()
returns trigger
language plpgsql
as $$
declare
    target_request_id uuid;
    current_count integer;
begin
    target_request_id = coalesce(new.analysis_request_id, old.analysis_request_id);

    select count(*)
    into current_count
    from public.analysis_assets
    where analysis_request_id = target_request_id;

    if current_count > 10 then
        raise exception 'analysis request cannot contain more than 10 assets';
    end if;

    update public.analysis_requests
    set assets_count = current_count
    where id = target_request_id;

    return null;
end;
$$;

drop trigger if exists analysis_assets_refresh_count_after_insert on public.analysis_assets;
create trigger analysis_assets_refresh_count_after_insert
after insert on public.analysis_assets
for each row execute function public.refresh_analysis_assets_count();

drop trigger if exists analysis_assets_refresh_count_after_delete on public.analysis_assets;
create trigger analysis_assets_refresh_count_after_delete
after delete on public.analysis_assets
for each row execute function public.refresh_analysis_assets_count();

alter table public.profiles enable row level security;
alter table public.analysis_requests enable row level security;
alter table public.analysis_assets enable row level security;
alter table public.asset_fundamentals_snapshot enable row level security;
alter table public.macro_indicators_snapshot enable row level security;
alter table public.ai_analysis_results enable row level security;
alter table public.usage_events enable row level security;

create policy "profiles_select_own"
on public.profiles for select
using (id = auth.uid());

create policy "profiles_update_own"
on public.profiles for update
using (id = auth.uid())
with check (id = auth.uid());

create policy "analysis_requests_select_own"
on public.analysis_requests for select
using (user_id = auth.uid());

create policy "analysis_requests_insert_own"
on public.analysis_requests for insert
with check (user_id = auth.uid());

create policy "analysis_assets_select_own"
on public.analysis_assets for select
using (
    exists (
        select 1
        from public.analysis_requests ar
        where ar.id = analysis_request_id
          and ar.user_id = auth.uid()
    )
);

create policy "asset_fundamentals_select_own"
on public.asset_fundamentals_snapshot for select
using (
    exists (
        select 1
        from public.analysis_assets aa
        join public.analysis_requests ar on ar.id = aa.analysis_request_id
        where aa.id = analysis_asset_id
          and ar.user_id = auth.uid()
    )
);

create policy "macro_indicators_select_own"
on public.macro_indicators_snapshot for select
using (
    exists (
        select 1
        from public.analysis_requests ar
        where ar.id = analysis_request_id
          and ar.user_id = auth.uid()
    )
);

create policy "ai_analysis_results_select_own"
on public.ai_analysis_results for select
using (
    exists (
        select 1
        from public.analysis_assets aa
        join public.analysis_requests ar on ar.id = aa.analysis_request_id
        where aa.id = analysis_asset_id
          and ar.user_id = auth.uid()
    )
);

create policy "usage_events_select_own"
on public.usage_events for select
using (user_id = auth.uid());
