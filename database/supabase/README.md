# Supabase Schema

Este diretorio contem os scripts SQL do Supabase PostgreSQL.

## Arquivos

- `0001_initial_schema.sql`: schema inicial do FinIA com tabelas, indices, triggers e politicas RLS.

## Regras Principais

- Cada analise pertence a um usuario autenticado.
- Cada analise aceita no maximo 10 papeis.
- Os dados retornados por Brapi, Banco Central e OpenAI sao salvos como snapshots.
- Identificadores reais do Supabase Auth nao devem ser gravados nas tabelas de negocio.
- A aplicacao grava `anonymized_user_id`, derivado por HMAC-SHA256 no backend.
- As politicas RLS esperam a claim `anonymized_user_id` quando houver acesso direto com JWT.

## Aplicacao Manual

Execute o conteudo do script no SQL Editor do Supabase ou por uma ferramenta de migracao aprovada para o projeto.
