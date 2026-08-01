# Ambiente e Segredos

Configure os segredos fora do Git.

## Azure Functions App Settings

- `AzureWebJobsStorage`
- `FUNCTIONS_WORKER_RUNTIME=dotnet-isolated`
- `BCB_BASE_URL=https://api.bcb.gov.br`
- `BRAPI_BASE_URL=https://brapi.dev`
- `BRAPI_TOKEN`
- `OPENAI_API_KEY`
- `OPENAI_MODEL=gpt-4o-mini`
- `SUPABASE_URL`
- `SUPABASE_JWT_SECRET`
- `SUPABASE_CONNECTION_STRING`
- `ANONYMIZATION_SECRET`
- `MAX_ASSETS_PER_ANALYSIS=10`
- `APPLICATIONINSIGHTS_CONNECTION_STRING`

## Anonimizacao

`ANONYMIZATION_SECRET` deve ser um segredo forte, estavel por ambiente e com pelo menos 32 caracteres. Ele e usado para gerar `anonymized_user_id` por HMAC-SHA256 antes de persistir dados de analise no banco.

## Smoke Tests

```powershell
dotnet restore FinIA.slnx
dotnet build FinIA.slnx --no-restore
dotnet test FinIA.slnx --no-build
```
