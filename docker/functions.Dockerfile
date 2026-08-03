FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FinIA.slnx ./
COPY src/FinIA.Domain/FinIA.Domain.csproj src/FinIA.Domain/
COPY src/FinIA.Application/FinIA.Application.csproj src/FinIA.Application/
COPY src/FinIA.Infrastructure/FinIA.Infrastructure.csproj src/FinIA.Infrastructure/
COPY src/FinIA.Functions/FinIA.Functions.csproj src/FinIA.Functions/

RUN dotnet restore src/FinIA.Functions/FinIA.Functions.csproj

COPY src/ src/
RUN dotnet publish src/FinIA.Functions/FinIA.Functions.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0
ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    AzureWebJobsScriptRoot=/home/site/wwwroot \
    FUNCTIONS_WORKER_RUNTIME=dotnet-isolated

COPY --from=build /app/publish /home/site/wwwroot
