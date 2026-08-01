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
