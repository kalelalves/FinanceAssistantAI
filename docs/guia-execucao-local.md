# Guia de Execucao Local - FinIA

Este guia mostra como preparar o ambiente e rodar a aplicacao localmente: Azure Functions no backend e Blazor WebAssembly no frontend.

## Pre-requisitos

- Windows com PowerShell.
- SDK .NET 10 instalado e disponivel no `PATH`.
- Azure Functions Core Tools instalado.
- Conta/projeto Supabase com PostgreSQL e Auth.
- Token da Brapi.
- Chave da OpenAI, opcional para desenvolvimento, pois a aplicacao possui fallback deterministico.

Verifique as ferramentas:

```powershell
dotnet --list-sdks
func --version
```

O projeto mira `net10.0`. Se apenas SDK .NET 9 ou inferior aparecer, `restore`, `build` e `test` falharao.

## 1. Restaurar dependencias

Na raiz do repositorio:

```powershell
cd C:\Users\Kalel\source\repos\FinanceAssistantAI
dotnet restore FinIA.slnx
```

## 2. Preparar o banco Supabase

No SQL Editor do Supabase, execute o script:

```text
database/supabase/0001_initial_schema.sql
```

O schema usa `anonymized_user_id` nas tabelas de negocio. O ID real do Supabase Auth nao deve ser persistido nessas tabelas.

## 3. Configurar segredos locais

Copie o exemplo de configuracao da Function:

```powershell
Copy-Item src\FinIA.Functions\local.settings.json.example src\FinIA.Functions\local.settings.json
```

Edite `src\FinIA.Functions\local.settings.json` e preencha:

```json
{
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "BCB_BASE_URL": "https://api.bcb.gov.br",
    "BRAPI_BASE_URL": "https://brapi.dev",
    "BRAPI_TOKEN": "seu-token-brapi",
    "OPENAI_API_KEY": "sua-chave-openai",
    "OPENAI_MODEL": "gpt-4o-mini",
    "SUPABASE_URL": "https://seu-projeto.supabase.co",
    "SUPABASE_JWT_SECRET": "jwt-secret-do-supabase",
    "SUPABASE_CONNECTION_STRING": "connection-string-postgres",
    "ANONYMIZATION_SECRET": "segredo-forte-com-pelo-menos-32-caracteres",
    "MAX_ASSETS_PER_ANALYSIS": "10"
  }
}
```

Notas:

- Nunca commitar `local.settings.json`.
- `ANONYMIZATION_SECRET` precisa ser estavel por ambiente. Se mudar, o mesmo usuario passara a gerar outro `anonymized_user_id`.
- `OPENAI_API_KEY` pode ficar vazia em desenvolvimento, mas o health ficara degradado e a analise usara fallback.

## 4. Rodar o backend

Opcao A, porta padrao do Azure Functions:

```powershell
cd src\FinIA.Functions
func start
```

API local:

```text
http://localhost:7071/api
```

Opcao B, porta do launch profile do projeto:

```powershell
cd src\FinIA.Functions
func start --port 7078
```

API local:

```text
http://localhost:7078/api
```

Teste o health:

```powershell
Invoke-RestMethod http://localhost:7071/api/health
```

Se usar a porta `7078`, ajuste a URL:

```powershell
Invoke-RestMethod http://localhost:7078/api/health
```

## 5. Obter um JWT do Supabase

A rota de analise exige:

```text
Authorization: Bearer <supabase-jwt>
```

Para teste manual, gere um usuario no Supabase Auth e use o access token da sessao autenticada. O `sub` do JWT deve ser um UUID valido.

## 6. Testar a API manualmente

Com o backend rodando:

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

Se usar porta `7078`, troque a URI para:

```text
http://localhost:7078/api/analyses
```

## 7. Rodar o frontend

Em outro terminal:

```powershell
cd C:\Users\Kalel\source\repos\FinanceAssistantAI
dotnet run --project src\FinIA.Web\FinIA.Web.csproj
```

URL local padrao:

```text
http://localhost:5027
```

Na tela do FinIA:

- Campo `API`: informe `http://localhost:7071` ou `http://localhost:7078`, conforme a porta usada no backend.
- Campo `Bearer token`: cole o JWT do Supabase.
- Campo `Tickers`: informe ate 10 papeis, por exemplo `PETR4, VALE3, ITUB4`.

## 8. Validar antes de commitar

Na raiz do repositorio:

```powershell
dotnet format FinIA.slnx --verify-no-changes
dotnet build FinIA.slnx --configuration Release
dotnet test FinIA.slnx --configuration Release
```

## Problemas comuns

### Erro NETSDK1045

Mensagem esperada quando o SDK .NET 10 nao esta instalado:

```text
The current .NET SDK does not support targeting .NET 10.0
```

Solucao: instalar o SDK .NET 10 e confirmar com `dotnet --list-sdks`.

### Health degradado

O health retorna degradado quando alguma configuracao obrigatoria esta ausente. Confira `local.settings.json`.

### Frontend nao conecta na API

Confirme:

- Backend esta rodando.
- Porta da API no frontend esta correta.
- CORS esta configurado no ambiente quando rodar fora do localhost.
- Token Supabase nao expirou.

### Analise retorna 401

Confirme:

- Header esta no formato `Authorization: Bearer <token>`.
- JWT foi emitido pelo Supabase correto.
- `SUPABASE_JWT_SECRET` corresponde ao projeto.

### Analise retorna erro de limite

A aplicacao aceita no maximo 10 papeis por requisicao.
