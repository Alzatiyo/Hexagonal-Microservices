# =====================================================================
# Dockerfile — AuditService
# Microservicio de Auditoría (MongoDB + RabbitMQ)
# Coloca este archivo en:  Hexagonal-Microservices/AuditService/Dockerfile
# =====================================================================

# ─── Build Stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY Domain/Domain.csproj               Domain/
COPY Application/Application.csproj     Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Api/Api.csproj                     Api/
RUN dotnet restore Api/Api.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish --no-restore

# ─── Runtime Stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
