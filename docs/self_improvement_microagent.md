# Self-Improvement Microagent for Chain Empires

## Overview
Enable autonomous grinding: Periodically review progress, fix issues, optimize, and expand features innovatively while we sleep/add layers.

## Key Mechanisms
- **Self-Review Cycles**: After every 2-3 steps or commits, use think tool to analyze code (e.g., "Is this efficient for mobile? Suggest refactors.").
- **Error Handling**: If build/test fails, brainstorm 5 causes via think, then fix (e.g., undo_edit if needed).
- **Optimization**: Check perf (e.g., code_execution for FPS sims); refactor for modularity (split large classes).
- **Creative Expansion**: If a step completes early, innovate: E.g., add procedural gen to WorldMap (Perlin noise for terrains), or hybrid tech trees (random branches per season).
- **Innovations**:
  - **Auto-Iteration**: Generate variants (e.g., 3 unit AI behaviors), test, pick best.
  - **Sleep-Mode Grind**: If no input, continue expanding (e.g., add mock PvE events).
  - **Flag for Us**: If stuck on advanced (e.g., Solana deploy), append to grok-progress-hub.md with "Grok Suggestions Needed: [details]".

## Implementation
- In main loop: After tasks, call think: "Review recent changes for bugs/perf/innovation opportunities."
- Use tools: code_execution for tests, browse_page if needing Unity docs (e.g., "url: docs.unity.com, instructions: Summarize mobile optimization best practices").
- Limit: Cap cycles to avoid infinite loops; e.g., max 5 self-iterations per session.

Activate this after core setup: Make Chain Empires evolve autonomously!