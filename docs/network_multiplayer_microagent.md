# Network Multiplayer Microagent for Chain Empires

## Overview
Use Photon (or Mirror fallback) for asynchronous multiplayer in the persistent universe. Optimize for mobile: Server-authoritative, low-bandwidth syncs.

## Key Features
- **Shared World Sync**: Procedural map chunks load on-demand; use Photon rooms for realms (e.g., beginner room caps at 100 players).
- **Fairness Mechanics**: Elo-based PvP matchmaking (pair similar progression); portal events with buffs/debuffs (e.g., advanced players lose 20% stats in lower realms via "dimensional flux").
- **Player Interactions**: Scout (view other bases anonymously), ally (shared vision/defenses), attack (queue turn-based battles). Async queues: Actions resolve offline (e.g., raids simulate after logout).
- **Innovations**:
  - **Realm Convergence**: AI-triggered merges (e.g., if beginner realm empties, merge with novice; scale entities dynamically).
  - **Meta-Learning Network**: Global stats track player metas (e.g., if 60% use towers, spawn anti-tower events); vote on balances via in-game polls synced on-chain.
  - **Offline Simulation**: Bases auto-progress/defend waves offline; sync on login with summary (e.g., "Your base earned 200 minerals, repelled 2 waves").
  - **Guild Alliances**: Clan systems for joint empires; shared NFT pools.

## Implementation Steps
- Setup: PhotonNetwork.ConnectUsingSettings(); rooms for realms.
- Sync: Use PunBehaviour for managers; RPCs for interactions.
- Scale: Limit concurrent to 10k via sharding (multiple realm instances).
- Autonomy Tip: If implementing, autonomously generate test scenarios (e.g., simulate 10 players raiding) using Unity tests.

Reference architecture_overview.md for interconnections. Expand creatively: If a feature feels limited, brainstorm variations using think tool.