

# Chain Empires: Fun-First Alpha

A 3rd-person strategy game focused on core gameplay with base building, resource management, and asynchronous combat optimized for mobile. This alpha version prioritizes fun-first design with chain-ready architecture for future integration.

## Game Concept Overview

Chain Empires offers an engaging 5-minute session loop: Build → Raid → Recover → Progress. Players manage resources, construct buildings, train units, and engage in strategic combat against AI factions in a deterministic, server-authoritative environment.

## Key Features (Alpha Focus)

- **Asynchronous Play**: 5-minute session loops with real-time timers
- **Base Building**: Strategic placement of harvesters, barracks, walls, and defenses
- **Resource Management**: Ore and energy harvesting with automated workers
- **Unit Combat**: Melee and ranged units with simple yet engaging tactics
- **AI Factions**: Dynamic enemy bases that feel alive 24/7
- **Chain-Ready Architecture**: Abstracted Web3 integration for future cosmetics-only economy

## Technical Stack (Alpha)

- **Engine**: Unity 2023 LTS (C#)
- **Multiplayer**: Mirror for server-authoritative networking
- **Networking**: Deterministic RNG with reconciliation
- **Backend**: .NET 8 minimal API for telemetry and persistence
- **Database**: Postgres for off-chain data, Redis for caching

## Development Plan (Alpha Roadmap)

1. Core Fun Prototype: Base-building, resource system, unit combat
2. Server & Networking: Mirror integration, deterministic simulation
3. AI Factions: GOAP/BehaviorTree agents with diplomacy system
4. Progression & Live-Ops: Tech tree, quests, event banners
5. Economy Scaffolding: Cosmetics-only economy framework
6. Chain Abstraction: Interface layer for future Web3 integration

## Getting Started

To set up the development environment:

```bash
# Clone the repository
git clone https://github.com/lxsolutions/ChainEmpires.git

# Navigate to project directory
cd ChainEmpires

# Open in Unity Hub (2023 LTS)
```

### Project Structure Overview

```
Assets/
├── Client/                # Client-specific code and UI
├── Shared/               # Cross-platform interfaces and DTOs
│   └── IChainService.cs  # Web3 abstraction interface
├── Scripts/              # Core game systems
│   ├── GameManager.cs    # Main game orchestrator (Web3 disabled)
│   ├── Telemetry/        # Analytics tracking system
│   └── GameSettings.cs   # Global configuration with IsWeb3Enabled=false
└── Scenes/
    └── Skirmish_Alpha.unity  # Core loop prototype scene

Server/                   # Headless server project (.NET 8)
Backend/                 # REST API for telemetry and persistence
.docker-compose.yml      # Local development environment setup
```

## Current Alpha Status

✅ **Project Setup Complete**: Assembly boundaries established, Web3 disabled globally
✅ **Chain Abstraction**: IChainService interface implemented with MockChainService
✅ **Telemetry System**: Basic event tracking wrapper created
✅ **Build Pipelines**: GitHub Actions workflows for Android and server builds
✅ **Docker Support**: Containerized development environment ready

### Next Steps (Core Loop Prototype)

1. Implement building types: Harvester, Barracks, Wall
2. Configure unit types: Melee and Ranged units
3. Set up AI enemy base with simple state machine
4. Integrate Mirror networking package
5. Complete Skirmish_Alpha.unity scene setup

## Contributing

Please focus on core gameplay improvements and submit pull requests for review. Follow our coding standards defined in `.editorconfig`.

## License

This project is licensed under the MIT License.