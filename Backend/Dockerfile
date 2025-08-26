








# Backend API Dockerfile
#
# Builds the Chain Empires .NET 8 minimal API for telemetry and persistence.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app/Backend

# Copy CSProj and restore as distinct layers
COPY Api/Api.csproj .
COPY NuGet.config .

RUN dotnet restore --configfile NuGet.config

# Copy everything else and build app
COPY . .
RUN dotnet publish -c Release -o ./publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV DATABASE_URL="Host=postgres;Username=admin;Password=securepassword;Database=chainempires" \
    REDIS_URL="redis://localhost:6379" \
    API_PORT=8082 \
    LOG_LEVEL=Information

COPY --from=build /app/Backend/publish .

EXPOSE 8082/tcp
HEALTHCHECK CMD curl -f http://localhost:8082/health || exit 1

ENTRYPOINT ["dotnet", "Api.dll"]






