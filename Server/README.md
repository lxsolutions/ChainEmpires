





# Chain Empires Server

## Overview
The Chain Empires headless server provides authoritative game logic for combat, resource management, and AI factions. It uses Mirror networking with deterministic simulation to ensure state integrity across clients.

## Project Structure

```
Server/
├── ChainEmpires.Server/          # Main server project (Unity or .NET)
│   ├── Controllers/              # Network controllers
│   ├── Models/                   # Game data models
│   ├── Services/                 # Business logic services
│   └── Tests/                    # Unit and integration tests
└── README.md                     # Server documentation
```

## Getting Started

### Prerequisites
- .NET 8 SDK installed
- Unity Hub (for Unity-based server)
- Docker (optional, for containerized deployment)

### Building the Server

#### Using .NET CLI:
```bash
cd /workspace/ChainEmpires/Server
dotnet restore ChainEmpires.Server.csproj
dotnet build --configuration Release
```

#### Using Unity (if using Unity headless):
1. Open the project in Unity Hub
2. Switch to "Headless" build target
3. Build for Linux Server

### Running Tests
```bash
cd /workspace/ChainEmpires/Server
dotnet test ChainEmpires.Server.Tests.csproj --configuration Release
```

## Configuration

The server uses environment variables and config files:

- `SERVER_PORT`: TCP port to listen on (default: 8081)
- `MAX_PLAYERS`: Maximum concurrent players (default: 50)
- `DETERMINISTIC_SEED`: Seed for RNG synchronization
- `LOG_LEVEL`: Verbosity of server logs

## Deployment Options

### Docker
A Dockerfile is provided for containerized deployment:
```bash
docker build -t chainempires/server .
docker run -p 8081:8081 -e MAX_PLAYERS=50 chainempires/server
```

### Kubernetes (Advanced)
Helm charts and Kubernetes manifests are available in `/ops/k8s`.

## Development Guidelines

1. **Deterministic Logic**: All game logic must be deterministic to ensure client-server consistency.
2. **Thread Safety**: Use async/await patterns for I/O operations.
3. **Testing**: Write unit tests for all business logic services.
4. **Documentation**: Keep API documentation up-to-date.

## Contributing

Please follow our coding standards and submit pull requests against the `main` branch.



