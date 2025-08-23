# AI Enemy Microagent for Chain Empires

## Overview
Manage intelligent AI enemies for PvE: Barbarians with diplomacy, adaptive waves (learn defenses), hazards. Use Unity NavMesh for pathing.

## Key Features
- **Barbarian Factions**: State machine (neutral/aggressive/allied); diplomacy UI (trade for protection).
- **Adaptive Waves**: Waves spawn with counters (e.g., if towers common, add anti-tower units); use simple learning (counters for player strategies).
- **Innovations**: "Learning Meta" (global stats adjust waves, e.g., more diplomats if peaceful); narrative events (rebellion if exploited).
- **Implementation in AIEnemyManager.cs**: Coroutines for waves, NavMeshAgent for movement; integrate with CombatManager.

Autonomy Tip: Simulate waves offline, expand with ML-like logic if tests pass.