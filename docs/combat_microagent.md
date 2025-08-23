# Combat Microagent for Chain Empires

## Overview
Turn-based/asynchronous combat blending RTS oversight, Diablo action, and tower defense. Auto-resolve unless player intervenes in 3rd-person.

## Key Features
- **Turn-Based Resolver**: Queue strategies (formations, abilities); simulate outcomes with RNG for fairness.
- **3rd-Person Intervention**: Zoom in to control heroes (Diablo-style skills: e.g., whirlwind attack).
- **Tower Defense Integration**: Waves auto-spawn (use AIEnvironmentManager); adaptive enemies (learn defenses, e.g., anti-tower units if over-reliant).
- **Innovations**: "Echo Battles" (sim risk-free for practice), narrative outcomes (win unlocks lore/NFT traits), cross-realm scaling (debuffs for fairness).
- **PvP/PvE**: Elo matchmaking; alliances for co-op raids.

## Implementation in CombatManager.cs
- Use Unity Physics for simulations.
- State Machine: Planning → Resolution → Intervention Option.
- Balance: Cap interventions per day for mobile.
- Test: Offline sims with debug UI.

Reference for Step 5+.