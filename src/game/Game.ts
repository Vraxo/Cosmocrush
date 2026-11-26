import * as THREE from 'three';
import { EffectComposer } from 'three/addons/postprocessing/EffectComposer.js';
import { RenderPass } from 'three/addons/postprocessing/RenderPass.js';
import { UnrealBloomPass } from 'three/addons/postprocessing/UnrealBloomPass.js';
import { OutputPass } from 'three/addons/postprocessing/OutputPass.js';
import { EnemyManager } from '../entities/EnemyManager';
import { Arena } from '../world/Arena';
import { CombatEffects } from '../rendering/CombatEffects';
import { Hud } from '../ui/Hud';
import { Input } from './Input';
import { Player } from './Player';

export class Game {
  private readonly renderer: THREE.WebGLRenderer;
  private readonly composer: EffectComposer;
  private readonly scene = new THREE.Scene();
  private readonly camera: THREE.PerspectiveCamera;
  private readonly input: Input;
  private readonly player: Player;
  private readonly arena: Arena;
  private readonly enemies: EnemyManager;
  private readonly effects: CombatEffects;
  private readonly hud: Hud;
  private readonly raycaster = new THREE.Raycaster();
  private lastTime = 0;
  private elapsed = 0;
  private running = false;
  private deployed = false;
  private fireCooldown = 0;
  private reloadTimer = 0;
  private waveBreakTimer = 0;
  private waveComplete = false;
  private ammo = 24;
  private reserveAmmo = 120;
  private fpsTimer = 0;
  private fpsFrames = 0;

  constructor(canvas: HTMLCanvasElement) {
    this.renderer = new THREE.WebGLRenderer({
      canvas,
      antialias: true,
      alpha: false,
      powerPreference: 'high-performance',
      stencil: false,
    });
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio || 1, 1.65));
    this.renderer.setSize(window.innerWidth, window.innerHeight);
    this.renderer.outputColorSpace = THREE.SRGBColorSpace;
    this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
    this.renderer.toneMappingExposure = 1.12;
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFShadowMap;

    this.scene.background = new THREE.Color(0x101827);
    this.scene.fog = new THREE.FogExp2(0x192438, 0.00335);

    this.camera = new THREE.PerspectiveCamera(64, window.innerWidth / window.innerHeight, 0.1, 650);
    this.camera.position.set(0, 4, 18);

    this.composer = new EffectComposer(this.renderer);
    this.composer.addPass(new RenderPass(this.scene, this.camera));
    this.composer.addPass(new UnrealBloomPass(
      new THREE.Vector2(window.innerWidth, window.innerHeight),
      0.48,
      0.42,
      0.78
    ));
    this.composer.addPass(new OutputPass());

    this.createLighting();

    this.arena = new Arena(this.scene);
    this.player = new Player(this.scene);
    this.player.position.set(0, this.arena.getHeight(0, 9), 9);
    this.player.group.position.copy(this.player.position);
    this.player.updateCamera(this.camera, 1 / 60);

    this.enemies = new EnemyManager(this.scene, (x, z) => this.arena.getHeight(x, z));
    this.effects = new CombatEffects(this.scene);
    this.input = new Input(canvas);
    this.hud = new Hud();
    this.hud.setVitals(this.player.health, this.player.shield);
    this.hud.setAmmo(this.ammo, this.reserveAmmo);
    this.hud.setEnemyCount(this.enemies.aliveCount, this.enemies.wave);

    this.input.onLockChange((locked) => {
      if (this.deployed) this.hud.setPaused(!locked);
    });

    window.addEventListener('cosmocrush:deploy', () => this.deploy());
    window.addEventListener('cosmocrush:resume', () => this.resumeMission());
    window.addEventListener('cosmocrush:continue', () => this.continueMission());
    window.addEventListener('resize', () => this.resize());
  }

  public start(): void {
    if (this.running) return;
    this.running = true;
    this.lastTime = performance.now();
    requestAnimationFrame(this.loop);
  }

  private readonly loop = (now: number): void => {
    if (!this.running) return;
    const rawDt = (now - this.lastTime) / 1000;
    this.lastTime = now;
    const dt = Math.min(rawDt, 0.05);
    this.elapsed += dt;

    this.update(dt);
    this.composer.render();
    this.input.endFrame();

    this.fpsTimer += dt;
    this.fpsFrames++;
    if (this.fpsTimer >= 0.5) {
      this.hud.setFps(this.fpsFrames / this.fpsTimer);
      this.fpsTimer = 0;
      this.fpsFrames = 0;
    }

    requestAnimationFrame(this.loop);
  };

  private update(dt: number): void {
    this.arena.update(dt, this.elapsed);
    this.player.update(dt, this.input, (x, z) => this.arena.getHeight(x, z));
    this.player.updateCamera(this.camera, dt);
    this.enemies.update(dt, this.player.position, () => {
      if (this.player.takeDamage(9)) {
        this.hud.setVitals(this.player.health, this.player.shield);
        this.hud.flashDamage();
        if (this.player.health <= 0) {
          document.exitPointerLock();
          this.hud.setPaused(true);
        }
      }
    });
    this.effects.update(dt);
    this.hud.update(dt);

    this.fireCooldown = Math.max(0, this.fireCooldown - dt);
    if (this.input.consumePress('KeyR')) this.beginReload();

    if (this.reloadTimer > 0) {
      this.reloadTimer -= dt;
      if (this.reloadTimer <= 0) this.finishReload();
    } else if (this.deployed && this.input.isFiring && this.fireCooldown <= 0 && this.player.health > 0) {
      this.fire();
    }

    if (this.enemies.aliveCount === 0 && !this.waveComplete) {
      this.waveBreakTimer += dt;
      if (this.waveBreakTimer >= 1.65) {
        this.waveComplete = true;
        this.hud.setComplete(true);
      }
    }

    this.hud.setEnemyCount(this.enemies.aliveCount, this.enemies.wave);
  }

  private fire(): void {
    if (this.ammo <= 0) {
      this.fireCooldown = 0.25;
      if (this.reserveAmmo > 0) this.beginReload();
      return;
    }

    this.ammo--;
    this.fireCooldown = 0.115;
    this.raycaster.setFromCamera(new THREE.Vector2(0, 0), this.camera);
    const hit = this.enemies.hitTest(this.raycaster.ray);
    const muzzle = this.player.getMuzzlePosition();
    const end = hit?.point ?? this.raycaster.ray.origin.clone().addScaledVector(this.raycaster.ray.direction, 95);

    this.effects.fire(muzzle, end);
    if (hit) {
      this.effects.impact(hit.point, 0xff6d64);
      this.enemies.damage(hit.enemyId, 35);
      this.hud.showHit();
    } else {
      const impactPoint = this.raycaster.ray.at(35 + Math.random() * 20, new THREE.Vector3());
      this.effects.impact(impactPoint, 0x9fc8db);
    }

    this.hud.setAmmo(this.ammo, this.reserveAmmo);
  }

  private beginReload(): void {
    if (this.reloadTimer > 0 || this.ammo >= 24 || this.reserveAmmo <= 0) return;
    this.reloadTimer = 1.15;
    this.hud.setReloading(true);
  }

  private finishReload(): void {
    const needed = 24 - this.ammo;
    const loaded = Math.min(needed, this.reserveAmmo);
    this.ammo += loaded;
    this.reserveAmmo -= loaded;
    this.reloadTimer = 0;
    this.hud.setReloading(false);
    this.hud.setAmmo(this.ammo, this.reserveAmmo);
  }

  private deploy(): void {
    if (this.deployed) return;
    this.deployed = true;
    this.hud.setDeployed(true);
    this.input.requestPointerLock();
  }

  private resumeMission(): void {
    if (this.player.health <= 0) {
      this.player.restore();
      this.hud.setVitals(this.player.health, this.player.shield);
    }
    this.input.requestPointerLock();
  }

  private continueMission(): void {
    if (!this.waveComplete) return;
    this.waveComplete = false;
    this.waveBreakTimer = 0;
    this.player.restore();
    this.hud.setVitals(this.player.health, this.player.shield);
    this.enemies.startNextWave();
    this.hud.setComplete(false);
  }

  private createLighting(): void {
    const hemisphere = new THREE.HemisphereLight(0x9ab6d9, 0x1b1b22, 1.7);
    this.scene.add(hemisphere);

    const key = new THREE.DirectionalLight(0xffd4aa, 3.1);
    key.position.set(-38, 58, 24);
    key.castShadow = true;
    key.shadow.mapSize.set(2048, 2048);
    key.shadow.camera.left = -62;
    key.shadow.camera.right = 62;
    key.shadow.camera.top = 62;
    key.shadow.camera.bottom = -62;
    key.shadow.camera.near = 1;
    key.shadow.camera.far = 160;
    key.shadow.bias = -0.00016;
    key.shadow.normalBias = 0.035;
    this.scene.add(key);
    this.scene.add(key.target);

    const coolFill = new THREE.DirectionalLight(0x82baff, 1.25);
    coolFill.position.set(42, 22, -56);
    this.scene.add(coolFill);

    const horizonFill = new THREE.PointLight(0x3d87d1, 32, 110, 2);
    horizonFill.position.set(16, 11, -70);
    this.scene.add(horizonFill);

    const warmRim = new THREE.PointLight(0xff854b, 24, 52, 2);
    warmRim.position.set(-34, 8, -20);
    this.scene.add(warmRim);
  }

  private resize(): void {
    const width = Math.max(1, window.innerWidth);
    const height = Math.max(1, window.innerHeight);
    this.camera.aspect = width / height;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(width, height);
    this.composer.setSize(width, height);
  }
}