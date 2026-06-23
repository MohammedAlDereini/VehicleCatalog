# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore using only the project files first so this layer is cached until a
# .csproj / nuget.config / Directory.Build.props changes.
COPY nuget.config Directory.Build.props ./
COPY VehicleCatalog.Handler/VehicleCatalog.Handler.csproj VehicleCatalog.Handler/
COPY VehicleCatalog.Web/VehicleCatalog.Web.csproj VehicleCatalog.Web/
RUN dotnet restore VehicleCatalog.Web/VehicleCatalog.Web.csproj

# Copy the rest of the source and publish a release build.
COPY . .
RUN dotnet publish VehicleCatalog.Web/VehicleCatalog.Web.csproj -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# curl is used by the container HEALTHCHECK below.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true
EXPOSE 8080

COPY --from=build /app/publish .

# Run as the image's built-in non-root user.
USER app

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "VehicleCatalog.Web.dll"]
