# Chain Empires Architecture Overview

## Core Systems and Interconnections
- **GameManager**: Central hub. Initializes and orchestrates all other managers. References: ResourceManager, BuildingManager, UnitManager, ProgressionManager, WorldMapManager, NetworkManager (upcoming), Web3Manager (upcoming).
- **ResourceManager**: Handles gathering, consumption, and storage. Interacts with: UnitManager (for workers harvesting), BuildingManager (for production buildings), WorldMapManager (resource nodes on map).
- **BuildingManager**: Manages construction/upgrades. Interacts with: ResourceManager (costs), UnitManager (builders), ProgressionManager (tech unlocks), WorldMapManager (placement on grid).
- **UnitManager**: Controls unit production/leveling/movement. Interacts with: BuildingManager (produced in barracks), WorldMapManager (pathfinding on map), CombatManager (upcoming for battles).
- **ProgressionManager**: Tracks eras/tech tree. Interacts with: All managers for unlocks (e.g., new buildings/units at higher eras), WorldMapManager (realm advancements).
- **WorldMapManager**: Handles shared world, realms, procedural elements. Interacts with: UnitManager (movement/exploration), BuildingManager (base placement), AIEnvironmentManager (upcoming for dynamic ecosystems).
- **Upcoming: NetworkManager** (Photon-based): Syncs multiplayer state. Interacts with: All managers for asynchronous actions (e.g., queue building over time, PvP matchmaking).
- **Upcoming: Web3Manager** (Solana integration): Handles NFTs/tokens. Interacts with: ResourceManager (token earnings), UnitManager/BuildingManager (NFT assets), ProgressionManager (staking for yields).
- **Upcoming: CombatManager**: Turn-based battles with tower defense waves. Interacts with: UnitManager (attacks), WorldMapManager (environmental threats), AIEnvironmentManager.
- **Upcoming: UIManager**: Handles 3rd-person/overhead camera switch, HUD. Interacts with: All managers for displays (e.g., resource UI, selection).

## Simplified UML (Markdown Diagram)
GameManager
├── ResourceManager ↔ BuildingManager ↔ UnitManager
│
├── ProgressionManager ↔ WorldMapManager ↔ AIEnvironmentManager (Dynamic Ecosystems)
│
├── NetworkManager (Multiplayer Sync) ↔ All Managers
│
├── Web3Manager (NFTs/Tokens) ↔ Resource/Unit/Building/Progression
│
└── UIManager (Camera/HUD) ↔ All Managers
CombatManager ↔ UnitManager + WorldMapManager


## Key Design Principles
- **Modularity**: Use interfaces (e.g., IManager) for easy swapping/testing.
- **Asynchronous Play**: All actions use timers (e.g., via Coroutines in Unity) for mobile offline progression.
- **Fairness in Realms**: Use Elo matchmaking in NetworkManager; debuff advanced players in cross-realm events.
- **Optimization**: Use object pooling for units/buildings; LOD for maps.
- **Web3 Optional**: Wrap blockchain calls in conditionals; fallback to off-chain for non-NFT users.

Reference this doc before implementing new features to ensure cohesion.