# UI and Camera Microagent for Chain Empires

## Overview
Mobile-optimized UI (Unity UI) with switchable cameras: Overhead for strategy (Supreme Commander zoom), 3rd-person for action (Diablo immersion).

## Key Features
- **Camera System**: Custom controller: Pinch-zoom transitions smoothly; gesture-drag for pan. Auto-switch: Zoom in triggers 3rd-person hero control.
- **HUD/UI**: Resource dashboard, minimap (fog of war), queue panels (buildings/units with timers). Touch-friendly: Drag-select units, tap-to-command.
- **Innovations**:
  - **Augmented Overlay**: AR mode (AR Foundation) for real-world base previews (e.g., scan table to "place" NFT building virtually).
  - **Narrative UI**: Branching story popups (e.g., "Ally with barbarians?") with dynamic art based on choices.
  - **Meta-UI**: Community poll overlays; Web3 wallet status (e.g., token balance ticker).
  - **Adaptive HUD**: Hides during idle; expands in combat with ability hotkeys (Diablo-inspired skill bar).

## Implementation Steps
- Camera: Cinemachine for smooth follows; script for mode switches.
- UI: Canvas with panels; EventSystem for touch.
- Optimize: 60FPS target; use Addressables for dynamic loading.
- Autonomy Tip: Autonomously prototype scenes (e.g., base view, battle zoom) and test on simulated mobile (Unity Device Simulator).

Integrate with UIManager.cs. Expand: If UI feels basic, innovate variants (e.g., customizable themes via NFTs).