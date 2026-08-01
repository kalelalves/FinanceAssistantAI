using FinIA.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinIA.Infrastructure.Persistence;

public sealed class FinIaDbContext(DbContextOptions<FinIaDbContext> options) : DbContext(options)
{
    public DbSet<AnalysisRequestEntity> AnalysisRequests => Set<AnalysisRequestEntity>();

    public DbSet<AnalysisAssetEntity> AnalysisAssets => Set<AnalysisAssetEntity>();

    public DbSet<AssetFundamentalsSnapshotEntity> AssetFundamentalsSnapshots => Set<AssetFundamentalsSnapshotEntity>();

    public DbSet<MacroIndicatorsSnapshotEntity> MacroIndicatorsSnapshots => Set<MacroIndicatorsSnapshotEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalysisRequestEntity>(entity =>
        {
            entity.ToTable("analysis_requests");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.AnonymizedUserId).HasColumnName("anonymized_user_id");
            entity.Property(x => x.Status).HasColumnName("status");
            entity.Property(x => x.AssetsCount).HasColumnName("assets_count");
            entity.Property(x => x.RequestedAt).HasColumnName("requested_at");
            entity.Property(x => x.CompletedAt).HasColumnName("completed_at");
            entity.Property(x => x.ErrorMessage).HasColumnName("error_message");
            entity.HasMany(x => x.Assets)
                .WithOne(x => x.AnalysisRequest)
                .HasForeignKey(x => x.AnalysisRequestId);
        });

        modelBuilder.Entity<AnalysisAssetEntity>(entity =>
        {
            entity.ToTable("analysis_assets");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.AnalysisRequestId).HasColumnName("analysis_request_id");
            entity.Property(x => x.Ticker).HasColumnName("ticker");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(x => new { x.AnalysisRequestId, x.Ticker }).IsUnique();
        });

        modelBuilder.Entity<AssetFundamentalsSnapshotEntity>(entity =>
        {
            entity.ToTable("asset_fundamentals_snapshot");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.AnalysisAssetId).HasColumnName("analysis_asset_id");
            entity.Property(x => x.RegularMarketPrice).HasColumnName("regular_market_price");
            entity.Property(x => x.DividendYield).HasColumnName("dividend_yield");
            entity.Property(x => x.PriceToEarnings).HasColumnName("price_to_earnings");
            entity.Property(x => x.Source).HasColumnName("source");
            entity.Property(x => x.RawPayload).HasColumnName("raw_payload");
            entity.Property(x => x.CapturedAt).HasColumnName("captured_at");
        });

        modelBuilder.Entity<MacroIndicatorsSnapshotEntity>(entity =>
        {
            entity.ToTable("macro_indicators_snapshot");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.AnalysisRequestId).HasColumnName("analysis_request_id");
            entity.Property(x => x.SelicMetaAnnual).HasColumnName("selic_meta_annual");
            entity.Property(x => x.IpcaMonthly).HasColumnName("ipca_monthly");
            entity.Property(x => x.Ipca12M).HasColumnName("ipca_12m");
            entity.Property(x => x.UsdPtaxSell).HasColumnName("usd_ptax_sell");
            entity.Property(x => x.SavingsMonthly).HasColumnName("savings_monthly");
            entity.Property(x => x.Source).HasColumnName("source");
            entity.Property(x => x.RawPayload).HasColumnName("raw_payload");
            entity.Property(x => x.CapturedAt).HasColumnName("captured_at");
        });
    }
}
