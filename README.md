# FinIA

FinIA e um MVP de assessor financeiro objetivo para analise fundamentalista de ate 10 papeis por consulta.

## Stack

- .NET 10
- Azure Functions Isolated Worker
- Blazor WebAssembly
- Supabase Auth e PostgreSQL
- Banco Central SGS
- Brapi.dev
- OpenAI

## Estrutura

```text
src/
  FinIA.Functions/
  FinIA.Application/
  FinIA.Domain/
  FinIA.Infrastructure/
  FinIA.Web/
tests/
  FinIA.Tests/
```

## Execucao Local

```powershell
dotnet restore FinIA.slnx
dotnet build FinIA.slnx
dotnet test FinIA.slnx
```

O projeto de Functions usa `local.settings.json` apenas localmente. Segredos reais devem ficar fora do Git.

## Backend

Copie `src/FinIA.Functions/local.settings.json.example` para `src/FinIA.Functions/local.settings.json` e preencha os valores locais.

```powershell
cd src/FinIA.Functions
func start
```

Health check:

```text
GET http://localhost:7071/api/health
```

Criar analise:

```text
POST http://localhost:7071/api/analyses
Authorization: Bearer <supabase-jwt>
Content-Type: application/json

{
  "tickers": ["PETR4", "VALE3"]
}
```

O endpoint rejeita requisicoes sem Bearer token valido e bloqueia mais de 10 tickers antes de qualquer chamada externa.

## Clientes Externos

- `IBcbClient`: consulta series SGS do Banco Central.
- `IBrapiClient`: consulta cotacao e fundamentos basicos via Brapi.dev.

Ambos usam `IHttpClientFactory` e timeouts curtos para evitar travar o fluxo de analise.

## Persistencia

O projeto usa EF Core com PostgreSQL/Supabase na Infrastructure. O schema SQL versionado fica em `database/supabase`.

## Motor Fundamentalista

O motor calcula score, preco-alvo base, horizonte e diagnostico preliminar antes da chamada para IA.
