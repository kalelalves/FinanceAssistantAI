FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/FinIA.Web/FinIA.Web.csproj src/FinIA.Web/
RUN dotnet restore src/FinIA.Web/FinIA.Web.csproj

COPY src/FinIA.Web/ src/FinIA.Web/
RUN dotnet publish src/FinIA.Web/FinIA.Web.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM nginx:1.27-alpine
COPY docker/web.nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
