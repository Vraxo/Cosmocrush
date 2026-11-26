import * as THREE from 'three';
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
import { clone } from 'three/addons/utils/SkeletonUtils.js';

export type GameAssetId =
  | 'marine-aiming'
  | 'marine-advancing'
  | 'alien-adult'
  | 'alien-juvenile';

export interface LoadedAsset {
  root: THREE.Group;
  animations: THREE.AnimationClip[];
}

interface AssetDefinition {
  url: string;
  source: string;
  license: 'CC0-1.0';
}

export const ASSET_SOURCES: Record<GameAssetId, AssetDefinition> = {
  'marine-aiming': {
    url: 'https://cdn.3dassets.dev/assets/28519/v1/model.glb',
    source: 'https://3dassets.dev/assets/arctic-station-fps-kit-defender-heavy-aiming-13ed73cb',
    license: 'CC0-1.0',
  },
  'marine-advancing': {
    url: 'https://cdn.3dassets.dev/assets/28520/v1/model.glb',
    source: 'https://3dassets.dev/assets/arctic-station-fps-kit-defender-heavy-advancing-eb97d1a1',
    license: 'CC0-1.0',
  },
  'alien-adult': {
    url: 'https://cdn.3dassets.dev/assets/21113/v1/model.glb',
    source: 'https://3dassets.dev/assets/xeno-species-and-alien-creatures-chitinous-insectoid-w-f987f1ff',
    license: 'CC0-1.0',
  },
  'alien-juvenile': {
    url: 'https://cdn.3dassets.dev/assets/21133/v1/model.glb',
    source: 'https://3dassets.dev/assets/xeno-species-and-alien-creatures-insectoid-juvenile-9f2aec48',
    license: 'CC0-1.0',
  },
};

/**
 * Loads licensed GLB models once, caches the source scene, and returns an
 * independent clone for each actor. SkeletonUtils.clone also supports future
 * skinned/animated production characters without changing gameplay code.
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
          const root = gltf.scene;
          root.traverse((object) => {
            if (object instanceof THREE.Mesh) {
              object.castShadow = true;
              object.receiveShadow = true;
              object.frustumCulled = true;
            }
          });
          resolve({ root, animations: gltf.animations });
        },
        undefined,
        (error) => {
          console.warn('[Cosmocrush] Optional model failed to load:', id, error);
          resolve(null);
        }
      );
    });

    this.cache.set(id, pending);
    return pending;
  }
}
