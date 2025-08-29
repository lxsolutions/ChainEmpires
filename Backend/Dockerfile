








# Backend API Dockerfile
#
# Builds the Chain Empires .NET 8 minimal API for tournament system.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy solution and project files
COPY Backend.sln .
COPY Api/ChainEmpires.Api.csproj ./Api/
COPY NuGet.config .

RUN dotnet restore --configfile NuGet.config

# Copy everything else and build app
COPY . .
RUN dotnet publish Api/ChainEmpires.Api.csproj -c Release -o ./publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Development \
    ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=chainempires_tournament;Username=postgres;Password=postgres" \
    ConnectionStrings__Redis="redis:6379" \
    ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080/tcp
HEALTHCHECK CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ChainEmpires.Api.dll"]






