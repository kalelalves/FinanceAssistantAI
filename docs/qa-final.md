# QA Final do MVP

Data: 2026-08-01

## Escopo Validado

- Solution .NET 10 com Azure Functions, Blazor WASM, Domain, Application, Infrastructure e Tests.
- Modelagem Supabase inicial.
- Autenticacao por Bearer/JWT Supabase.
- Limite de 10 papeis por analise.
- Clientes externos BCB e Brapi.
- Persistencia inicial de analises.
- Motor fundamentalista.
- Integracao OpenAI com fallback.
- Endpoint publico `POST /api/analyses`.
- Frontend Blazor WASM.
- CI e documentacao operacional.

## Comandos Executados

```powershell
dotnet restore FinIA.slnx
dotnet build FinIA.slnx --no-restore
dotnet test FinIA.slnx --no-build
dotnet format FinIA.slnx --verify-no-changes
```

## Resultado Esperado

- Build sem erros.
- Testes automatizados passando.
- Format sem alteracoes pendentes.

## Riscos Restantes

- Validacao end-to-end real depende de segredos reais: Supabase, Brapi e OpenAI.
- O `gh` local continua sem autenticacao, mas os PRs foram abertos pelo conector GitHub.
- As fases estao em PRs encadeados porque as anteriores ainda nao foram mergeadas.

## Checklist Manual Recomendado

- Configurar segredos em ambiente local ou Azure.
- Aplicar `database/supabase/0001_initial_schema.sql` no Supabase.
- Gerar JWT real via Supabase Auth.
- Executar `GET /api/health`.
- Executar `POST /api/analyses` com 1 papel.
- Executar `POST /api/analyses` com 10 papeis.
- Confirmar bloqueio com 11 papeis.
- Abrir frontend Blazor WASM e executar o fluxo.
