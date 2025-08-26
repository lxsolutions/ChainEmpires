







# Server Dockerfile
#
# Builds the Chain Empires headless server using .NET 8 SDK.
# This can be used for both Unity-based and pure .NET implementations.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app/Server

# Copy CSProj and restore as distinct layers
COPY ChainEmpires.Server.csproj .
COPY NuGet.config .

RUN dotnet restore --configfile NuGet.config

# Copy everything else and build app
COPY . .
RUN dotnet publish -c Release -o ./publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV SERVER_PORT=8081 \
    MAX_PLAYERS=50 \
    DETERMINISTIC_SEED=42 \
    LOG_LEVEL=Information

COPY --from=build /app/Server/publish .

EXPOSE 8081/tcp
EXPOSE 8081/udp

ENTRYPOINT ["dotnet", "ChainEmpires.Server.dll"]





