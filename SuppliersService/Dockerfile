# =====================================================================
# Dockerfile — SuppliersService
# Microservicio de Proveedores (SQL Server + RabbitMQ)
# Coloca este archivo en:  Hexagonal-Microservices/SuppliersService/Dockerfile
# =====================================================================

# ─── Build Stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Eliminar referencias a carpetas NuGet de Windows (Visual Studio)
ENV NUGET_FALLBACK_PACKAGES=""

# Copiar archivos de proyecto y restaurar dependencias
COPY Domain/Domain.csproj           Domain/
COPY Aplication/Aplication.csproj   Aplication/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Api/Api.csproj                 Api/
RUN dotnet restore Api/Api.csproj --force

# Copiar todo el código y compilar
COPY . .
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

# ─── Runtime Stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
