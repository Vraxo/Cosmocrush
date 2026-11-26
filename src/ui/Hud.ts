export class Hud {
  private readonly healthNumber = this.element('health-number');
  private readonly shieldNumber = this.element('shield-number');
  private readonly healthFill = this.element('health-fill');
  private readonly shieldFill = this.element('shield-fill');
  private readonly ammoCurrent = this.element('ammo-current');
  private readonly ammoReserve = this.element('ammo-reserve');
  private readonly enemyCount = this.element('enemy-count');
  private readonly objectiveProgress = this.element('objective-progress');
  private readonly fpsValue = this.element('fps-value');
  private readonly damageFlash = this.element('damage-flash');
  private readonly hitMarker = this.element('hit-marker');
  private readonly reloadIndicator = this.element('reload-indicator');
  private readonly introLayer = this.element('intro-layer');
  private readonly pauseLayer = this.element('pause-layer');
  private readonly completeLayer = this.element('mission-complete');
  private readonly waveLabel = this.element('wave-label');
  private damageTimer = 0;
  private hitTimer = 0;

  constructor() {
    this.element('deploy-button').addEventListener('click', () => {
      window.dispatchEvent(new Event('cosmocrush:deploy'));
    });
    this.element('resume-button').addEventListener('click', () => {
      window.dispatchEvent(new Event('cosmocrush:resume'));
    });
    this.element('continue-button').addEventListener('click', () => {
      window.dispatchEvent(new Event('cosmocrush:continue'));
    });
  }

  public setVitals(health: number, shield: number): void {
    this.healthNumber.textContent = Math.ceil(health).toString();
    this.shieldNumber.textContent = Math.ceil(shield).toString();
    this.healthFill.style.width = Math.max(0, health) + '%';
    this.shieldFill.style.width = Math.max(0, shield / 75 * 100) + '%';
  }

  public setAmmo(current: number, reserve: number): void {
    this.ammoCurrent.textContent = current.toString().padStart(2, '0');
    this.ammoReserve.textContent = reserve.toString().padStart(3, '0');
  }

  public setEnemyCount(count: number, wave: number): void {
    this.enemyCount.textContent = count.toString().padStart(2, '0');
    this.objectiveProgress.style.width = ((10 - Math.min(10, count)) / 10 * 100) + '%';
    this.waveLabel.textContent = count > 0 ? 'HOSTILES INBOUND // WAVE ' + wave : 'SECTOR TEMPORARILY CLEAR';
  }

  public setFps(fps: number): void {
    this.fpsValue.textContent = Math.round(fps).toString();
  }

  public setDeployed(deployed: boolean): void {
    this.introLayer.classList.toggle('is-hidden', deployed);
  }

  public setPaused(paused: boolean): void {
    this.pauseLayer.classList.toggle('is-hidden', !paused);
  }

  public setComplete(complete: boolean): void {
    this.completeLayer.classList.toggle('is-hidden', !complete);
  }

  public setReloading(reloading: boolean): void {
    this.reloadIndicator.classList.toggle('is-hidden', !reloading);
  }

  public flashDamage(): void {
    this.damageTimer = 0.28;
    this.damageFlash.classList.add('active');
  }

  public showHit(): void {
    this.hitTimer = 0.12;
    this.hitMarker.classList.add('active');
  }

  public update(dt: number): void {
    if (this.damageTimer > 0) {
      this.damageTimer -= dt;
      if (this.damageTimer <= 0) this.damageFlash.classList.remove('active');
    }

    if (this.hitTimer > 0) {
      this.hitTimer -= dt;
      if (this.hitTimer <= 0) this.hitMarker.classList.remove('active');
    }
  }

  private element(id: string): HTMLElement {
    const element = document.getElementById(id);
    if (!element) throw new Error('Missing HUD element: #' + id);
    return element;
  }
}