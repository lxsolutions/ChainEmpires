






# Chain Empires Backend

## Overview
The Chain Empires backend provides REST APIs for telemetry, player data persistence, and game state management. It uses .NET 8 minimal API with PostgreSQL for persistent storage.

## Project Structure

```
Backend/
├── Api/                          # Main API project (.NET)
│   ├── Controllers/              # HTTP endpoints
│   ├── Services/                 # Business logic services
│   └── Models/                   # Data models
└── README.md                     # Backend documentation
```

## Getting Started

### Prerequisites
- .NET 8 SDK installed
- PostgreSQL database server
- Redis cache (optional)

### Building the API

```bash
cd /workspace/ChainEmpires/Backend
dotnet restore Api/Api.csproj
dotnet build --configuration Release
```

### Running Migrations

```bash
cd /workspace/ChainEmpires/Backend
dotnet ef database update --project Api/Api.csproj
```

## Configuration

The API uses environment variables for configuration:

- `DATABASE_URL`: PostgreSQL connection string
- `REDIS_URL`: Redis cache URL (optional)
- `API_PORT`: HTTP port to listen on (default: 8082)

## Endpoints

### Telemetry
- `POST /events` - Receive game telemetry events
- `GET /metrics` - Get aggregated player metrics

### Player Data
- `GET /players/{id}` - Retrieve player profile
- `PUT /players/{id}/inventory` - Update player inventory

## Deployment Options

### Docker
A Dockerfile is provided for containerized deployment:
```bash
docker build -t chainempires/api .
docker run -p 8082:8082 \
  -e DATABASE_URL="Host=postgres;Database=chainempires" \
  -e REDIS_URL="redis://localhost:6379" \
  chainempires/api
```

### Kubernetes (Advanced)
Helm charts and Kubernetes manifests are available in `/ops/k8s`.

## Development Guidelines

1. **API Versioning**: Use versioned endpoints (`/v1/events`)
2. **Security**: Implement proper authentication for sensitive endpoints
3. **Testing**: Write integration tests for all API endpoints
4. **Documentation**: Keep OpenAPI/Swagger documentation up-to-date

## Contributing

Please follow our coding standards and submit pull requests against the `main` branch.




