# Guia Docker Local - FinIA

Este guia sobe a aplicacao localmente com Docker Compose:

- `finia-api`: Azure Functions .NET isolated.
- `finia-web`: Blazor WebAssembly publicado em Nginx.
- `postgres`: banco local com o schema do projeto.
- `azurite`: storage local usado pelo runtime da Azure Functions.

## Pre-requisitos

- Docker Desktop.
- SDK .NET 10 nao e necessario para rodar via Docker, mas as imagens `mcr.microsoft.com/dotnet/sdk:10.0` e `mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0` precisam estar disponiveis no registry.

## 1. Criar arquivo de ambiente

Na raiz do repositorio:

```powershell
Copy-Item .env.example .env
```

Edite `.env` e preencha pelo menos:

```text
BRAPI_TOKEN=...
SUPABASE_URL=https://seu-projeto.supabase.co
SUPABASE_JWT_SECRET=...
ANONYMIZATION_SECRET=segredo-forte-com-pelo-menos-32-caracteres
```

`OPENAI_API_KEY` pode ficar vazio em desenvolvimento. Nesse caso, a aplicacao usa o fallback deterministico.

## 2. Subir containers

```powershell
docker compose up --build
```

Servicos:

- Frontend: `http://localhost:5027`
- Backend direto: `http://localhost:7071/api`
- Backend via frontend: `http://localhost:5027/api`
- Postgres: `localhost:5432`
- Azurite Blob: `localhost:10000`
- Azurite Queue: `localhost:10001`
- Azurite Table: `localhost:10002`

## 3. Testar health

Em outro terminal:

```powershell
Invoke-RestMethod http://localhost:7071/api/health
Invoke-RestMethod http://localhost:5027/api/health
```

Se algum segredo obrigatorio estiver ausente, o health pode retornar degradado.

## 4. Usar o frontend

Abra:

```text
http://localhost:5027
```

Na tela:

- Chat: digite ate 10 papeis, por exemplo `Analise PETR4 e VALE3`.
- O usuario nao configura API ou token no frontend.

## 5. Testar API manualmente

```powershell
$token = "<supabase-jwt>"
$headers = @{
  Authorization = "Bearer $token"
  "Content-Type" = "application/json"
}
$body = @{
  tickers = @("PETR4", "VALE3")
} | ConvertTo-Json

Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:7071/api/analyses" `
  -Headers $headers `
  -Body $body
```

## 6. Parar e limpar

Parar containers:

```powershell
docker compose down
```

Parar e apagar volumes locais:

```powershell
docker compose down -v
```

Use `-v` quando quiser recriar o banco do zero e reaplicar `database/supabase/0001_initial_schema.sql`.

## Problemas comuns

### Imagem .NET 10 nao encontrada

Confirme se as imagens .NET 10 ja estao disponiveis no registry usado pelo Docker:

```powershell
docker pull mcr.microsoft.com/dotnet/sdk:10.0
docker pull mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0
```

### API nao sobe por erro de storage

Confirme se o container `azurite` esta rodando:

```powershell
docker compose ps
```

O Compose usa a conta padrao local `devstoreaccount1` do Azurite.

### Banco nao tem tabelas

Se o volume ja existia antes da criacao do schema, o script de init nao roda novamente. Recrie os volumes:

```powershell
docker compose down -v
docker compose up --build
```

### Erro 401 na analise

O endpoint tecnico `/api/analyses` depende de um JWT Supabase valido e do `SUPABASE_JWT_SECRET` correto.
No frontend, use o chat em `/api/chat`.
