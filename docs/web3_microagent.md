# Web3 Integration Microagent for Chain Empires

## Overview
Integrate Solana for low-fee NFTs/tokens. Make optional: Non-Web3 players use simulated assets; Web3 users connect wallets for true ownership/P2E.

## Key Components
- **Wallet Integration**: Use Solana Unity SDK for Phantom wallet connect. UI button in settings to link wallet.
- **Native Token ("Chain Crystals")**: SPL token on Solana. Smart contract for minting/burning.
  - Earn: Quests (daily logins = 10 tokens), PvP wins (stake on battles), defending waves (tower defense bonuses), staking bases (yield farming).
  - Uses: Speed timers (e.g., halve building upgrade time for 5 tokens), buy resources, governance votes on events.
  - Balance P2E: Cap daily earnings; free grind path equals paid efficiency.
- **NFT Assets**:
  - **Land Plots**: Mint as NFTs with attributes (yield multiplier, location). Passive income: Auto-generate resources offline. Trade on OpenSea-like markets.
  - **Units/Heroes**: ERC-721 style on Solana. Rarity tiers (common-legendary). Breeding: Combine two NFTs for offspring (e.g., warrior + archer = hybrid). Evolution: Level up in-game mutates traits (e.g., +fire damage after 10 waves defended).
  - **Buildings/Blueprints**: Modular NFTs. Mix parts (e.g., NFT tower base + roof) for custom bonuses. Rarity affects efficiency (e.g., legendary harvester = +50% yield).
  - **Empire Bundles**: Sell entire base as NFT (serializes progression data on-chain).
- **Innovative Features**:
  - **Dynamic Evolution**: NFTs change based on world events (e.g., during "comet impact" event, units gain cosmic traits if exposed).
  - **Cross-Realm Portals**: NFTs transferable between realms with buffs/debuffs (e.g., galactic hero debuffed in stone age).
  - **Governance DAO**: Token holders vote on metas (e.g., new unit types) via on-chain proposals.
  - **Betting/Staking Wars**: Spectator mode for PvP; stake tokens on outcomes.
  - **AR NFT Viewer**: Use AR Foundation to visualize owned NFTs in real world (optional mobile feature).

## Implementation Steps
1. Set up Solana SDK: Import and configure in Web3Manager.cs.
2. Wallet UI: Script for connect/disconnect; store public key.
3. Token Contract: Deploy simple SPL token (use Solana tools locally first).
4. NFT Minting: Functions to mint/transfer (e.g., MintHeroNFT(unitData)).
5. Sync: Off-chain gameplay syncs to on-chain periodically (e.g., level ups update NFT metadata).
6. Anti-P2W: NFTs give utility/cosmetics only; core power from grind.

## Potential Pitfalls & Fixes
- Gas Fees: Use Solana for ~$0.000005/tx.
- Security: Validate all on-chain calls server-side (Firebase for auth).
- Fallback: If wallet not connected, use local JSON sims for "NFTs".

Update this microagent as integration progresses.