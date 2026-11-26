# Cosmocrush

A TypeScript + Three.js sci-fi shooter prototype. The first playable slice is **Outpost Seven**: a third-person combat encounter on a hostile frontier world, built around a cinematic alien-outpost scene.

## Run locally

Requires Node.js 20.19+ or 22.12+.

```sh
npm install
npm run dev
```

Open the local Vite URL, select **Deploy to Battlefield**, and click the scene to capture the mouse.

## Controls

- **W / A / S / D** — move
- **Mouse** — aim
- **Left mouse button** — fire
- **Shift** — sprint
- **R** — reload
- **Escape** — release the pointer / pause

## Current slice

- Procedural rocky terrain, debris fields, and a modular frontier outpost
- Planetary sky, stars, distant ships, atmospheric fog, and cinematic lighting
- Third-person armored marine and animated procedural alien swarm
- Hitscan rifle, tracers, hit feedback, shield and suit damage, ammunition, and waves
- Responsive mission HUD and pointer-lock controls

The initial characters and environment are procedural art placeholders. The renderer and scene are intentionally structured to support production GLB assets, authored animations, PBR texture sets, and more advanced effects without tying gameplay to a particular asset source.

## Architecture

- `src/game` — frame loop, input, and player
- `src/world` — arena, lighting, terrain, and large environment set pieces
- `src/entities` — enemy construction and swarm behavior
- `src/ui` — DOM HUD
- `src/rendering` — post-processing and temporary combat effects

## Quality goals

The first pass targets a stable, scalable 1440p desktop experience. Rendering quality is configurable in one place; the scene does not depend on Caveria's voxel world, chunk mesher, or block materials.

## License

MIT.