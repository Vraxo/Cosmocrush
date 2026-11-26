# Cosmocrush

A TypeScript + Three.js sci-fi shooter prototype. **Outpost Seven** is a cinematic frontier-world defense encounter: survive the first alien swarm, clear the landing perimeter, and hold the line.

## Run locally

Requires Node.js 20.19+ or 22.12+.

```sh
npm install
npm run dev
```

Open the Vite URL, select **Deploy to Battlefield**, and click the scene to capture the mouse.

## Controls

- **W / A / S / D** — move
- **Mouse** — aim
- **Left mouse button** — fire
- **Shift** — sprint
- **R** — reload
- **Escape** — release the pointer / pause

## Graphics pipeline

- Three.js 0.187 with ACES tone mapping, soft shadow maps, PMREM environment lighting, GTAO, bloom, and SMAA.
- Layered rocky terrain with generated albedo, normal, and roughness maps; instanced rock fields, cliffs, debris, and impact craters.
- Modular outpost geometry with PBR armor surfaces, pressure conduits, vent banks, beacons, and emissive lighting.
- Planetary sky, star field, distant spacecraft, atmospheric fog, and floating dust.
- Production GLB model loader with cached, independently cloned scenes and automatic procedural fallbacks.

## External model assets

The game attempts to load CC0-licensed models from the 3DAssets.dev CDN. A network connection is needed for the upgraded character models; if those downloads fail, the original local procedural models remain playable. Sources and license details are recorded in [assets/ATTRIBUTIONS.md](assets/ATTRIBUTIONS.md).

The current imported gunner and alien models are static-pose meshes, not fully rigged animation assets. The gunner switches between separate aiming and advancing poses. The next asset pass should replace these with rigged characters and authored locomotion / combat clips.

## Architecture

- `src/game` — frame loop, input, player movement, combat, and camera
- `src/world` — arena layout, terrain, outpost, and planetary set pieces
- `src/entities` — alien models, pursuit, hit handling, and wave spawning
- `src/assets` — model registry, CDN loading, caching, and scene cloning
- `src/rendering` — environment lighting, surface maps, post-processing support, dust, and combat effects
- `src/ui` — DOM-based mission HUD

The new game is independent of Caveria's voxel world, chunk mesher, and block materials.

## Build

```sh
npm run check
npm run build
```

## License

MIT for the project code. Third-party model assets retain their CC0 1.0 license as recorded in the asset register.
