import * as THREE from 'three';
import { clone } from 'three/addons/utils/SkeletonUtils.js';
import { LoadedAsset } from '../assets/AssetLibrary';
import { AlienEnemy } from './Enemy';

export interface EnemyHit {
  enemyId: number;
  point: THREE.Vector3;
}

export class EnemyManager {
  private readonly enemies: AlienEnemy[] = [];
  private nextId = 1;
  private randomState = 7281;
  private stalkerAsset: LoadedAsset | null = null;
  private lurkerAsset: LoadedAsset | null = null;
  public wave = 1;

  constructor(private readonly scene: THREE.Scene, private readonly groundHeight: (x: number, z: number) => number) {
    this.spawnWave(10);
  }

  public setProductionModels(stalker: LoadedAsset | null, lurker: LoadedAsset | null): void {
    this.stalkerAsset = stalker;
    this.lurkerAsset = lurker;

    for (const enemy of this.enemies) {
      const source = this.assetFor(enemy.variant);
      if (source) enemy.setProductionModel(clone(source.root) as THREE.Group, source.animations);
    }
  }

  public get aliveCount(): number {
    let count = 0;
    for (const enemy of this.enemies) if (enemy.alive) count++;
    return count;
  }

  public update(dt: number, playerPosition: THREE.Vector3, onAttack: () => void): void {
    for (let i = this.enemies.length - 1; i >= 0; i--) {
      const enemy = this.enemies[i];
      enemy.update(dt, playerPosition, this.groundHeight, onAttack);
      if (!enemy.alive && enemy.deathTimer <= 0) {
        enemy.dispose(this.scene);
        this.enemies.splice(i, 1);
      }
    }
  }

  public hitTest(ray: THREE.Ray): EnemyHit | null {
    let closest: EnemyHit | null = null;
    let closestDistance = Number.POSITIVE_INFINITY;
    const point = new THREE.Vector3();

    for (const enemy of this.enemies) {
      if (!enemy.alive) continue;
      const hit = ray.intersectSphere(enemy.hitSphere, point);
      if (!hit) continue;
      const distance = ray.origin.distanceTo(hit);
      if (distance < closestDistance && distance < 100) {
        closestDistance = distance;
        closest = { enemyId: enemy.id, point: hit.clone() };
      }
    }
    return closest;
  }

  public damage(enemyId: number, amount: number): boolean {
    const enemy = this.enemies.find((candidate) => candidate.id === enemyId);
    return enemy?.damage(amount) ?? false;
  }

  public startNextWave(): void {
    this.wave++;
    this.spawnWave(Math.min(18, 8 + this.wave * 2));
  }

  private spawnWave(count: number): void {
    for (let i = 0; i < count; i++) {
      const angle = (i / count) * Math.PI * 2 + this.random() * 0.5;
      const radius = 18 + this.random() * 18;
      const x = Math.cos(angle) * radius;
      const z = 8 - Math.sin(angle) * radius;
      const variant = this.random() > 0.84 ? 2 : this.random() > 0.54 ? 1 : 0;
      const enemy = new AlienEnemy(
        this.scene,
        this.nextId++,
        x,
        this.groundHeight(x, z),
        z,
        variant
      );
      const source = this.assetFor(variant);
      if (source) enemy.setProductionModel(clone(source.root) as THREE.Group, source.animations);
      this.enemies.push(enemy);
    }
  }

  private assetFor(variant: number): LoadedAsset | null {
    if (variant === 1) return this.lurkerAsset ?? this.stalkerAsset;
    return this.stalkerAsset ?? this.lurkerAsset;
  }

  private random(): number {
    this.randomState = (this.randomState * 1664525 + 1013904223) >>> 0;
    return this.randomState / 4294967296;
  }
}