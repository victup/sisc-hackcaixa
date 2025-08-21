# Etapa base - runtime do .NET
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Cria um usuário e grupo não-root
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Troca para o usuário seguro
USER appuser

EXPOSE 8080

# Etapa build - SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SISC.csproj", "."]
RUN dotnet restore "./SISC.csproj"
COPY . .
RUN dotnet build "./SISC.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa publish - publica artefatos prontos
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SISC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final - roda com usuário não-root
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Local onde o SQLite vai gravar o banco
VOLUME /app/data

# cria a pasta /app/data para armazenar o SQLite
USER root
RUN mkdir -p /app/data && chown appuser:appgroup /app/data
USER appuser

ENTRYPOINT ["dotnet", "SISC.dll"]
