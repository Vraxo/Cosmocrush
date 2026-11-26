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

- Three.js 0.187 with ACES tone mapping, soft shadow maps, PMREM environment lighting, atmospheric fog, and floating dust.
- Layered rocky terrain with generated albedo, normal, and roughness maps; instanced rock fields, cliffs, debris, and impact craters.
- Modular outpost geometry with PBR armor surfaces, pressure conduits, vent banks, beacons, and restrained emissive details.
- Planetary sky, star field, distant spacecraft, and a subdued atmospheric rim.
- A locally built, articulated marine rig with continuous idle / walk / sprint motion, a supported rifle pose, and firing recoil.
- Rigged alien GLBs with independent animation mixers and blended idle / walk / run / attack / death clips; local procedural fallback when model downloads fail.

## External model assets

The game loads the **Blind Stalker** and **Deep Lurker** animated creatures from vinish.dev. They are CC BY 4.0 assets and include authored Idle, Walk, Run, Attack, Roar, and Die clips. Each enemy receives an independent skeleton clone and animation mixer. A network connection is required for these upgraded models; the local articulated alien remains as a fallback. Model credits and source links are recorded in [assets/ATTRIBUTIONS.md](assets/ATTRIBUTIONS.md).

The marine is built from local articulated armor meshes rather than switching between unrelated static poses. Its locomotion, ready stance, and firing recoil are updated continuously from gameplay input.

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
