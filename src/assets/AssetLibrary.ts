import * as THREE from 'three';
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
import { clone } from 'three/addons/utils/SkeletonUtils.js';

export type GameAssetId = 'marine-sci-fi-trooper' | 'alien-blind-stalker' | 'alien-deep-lurker';

export interface LoadedAsset {
  root: THREE.Group;
  animations: THREE.AnimationClip[];
}

interface AssetDefinition {
  url: string;
  source: string;
  license: 'CC-BY-4.0';
}

export const ASSET_SOURCES: Record<GameAssetId, AssetDefinition> = {
  'marine-sci-fi-trooper': {
    url: 'https://vinish-models.pages.dev/sci-fi-soldiers/sci-fi-trooper.glb',
    source: 'https://vinish.dev/free-3d-models/sci-fi-soldiers',
    license: 'CC-BY-4.0',
  },
  'alien-blind-stalker': {
    url: 'https://vinish-models.pages.dev/monsters/blind-stalker.glb',
    source: 'https://vinish.dev/free-3d-models/monsters',
    license: 'CC-BY-4.0',
  },
  'alien-deep-lurker': {
    url: 'https://vinish-models.pages.dev/monsters/deep-lurker.glb',
    source: 'https://vinish.dev/free-3d-models/monsters',
    license: 'CC-BY-4.0',
  },
};

/**
 * Loads licensed, animated GLB creatures once, then returns independent
 * skeleton-safe clones so every enemy owns its own animation mixer.
 */
export class AssetLibrary {
  private readonly loader = new GLTFLoader();
  private readonly cache = new Map<GameAssetId, Promise<LoadedAsset | null>>();

  public async instantiate(id: GameAssetId): Promise<LoadedAsset | null> {
    const source = this.cache.get(id) ?? this.beginLoad(id);
    const asset = await source;
    if (!asset) return null;

    return {
      root: clone(asset.root) as THREE.Group,
      animations: [...asset.animations],
    };
  }

  private beginLoad(id: GameAssetId): Promise<LoadedAsset | null> {
    const definition = ASSET_SOURCES[id];
    const pending = new Promise<LoadedAsset | null>((resolve) => {
      this.loader.load(
        definition.url,
        (gltf) => {
          gltf.scene.traverse((object) => {
            if (object instanceof THREE.Mesh) {
              object.castShadow = true;
              object.receiveShadow = true;
              if (object instanceof THREE.SkinnedMesh) object.frustumCulled = false;
            }
          });
          resolve({ root: gltf.scene, animations: gltf.animations });
        },
        undefined,
        (error) => {
          console.warn('[Cosmocrush] Optional animated creature failed to load:', id, error);
          resolve(null);
        }
      );
    });

    this.cache.set(id, pending);
    return pending;
  }
}