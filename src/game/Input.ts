export class Input {
  private readonly keys = new Set<string>();
  private readonly pressed = new Set<string>();
  private mouseDown = false;
  private mouseDeltaX = 0;
  private locked = false;
  private readonly lockListeners = new Set<(locked: boolean) => void>();

  constructor(private readonly canvas: HTMLCanvasElement) {
    window.addEventListener('keydown', (event) => {
      if (!event.repeat) this.pressed.add(event.code);
      this.keys.add(event.code);
      if (event.code === 'Space' || event.code.startsWith('Arrow')) event.preventDefault();
    });

    window.addEventListener('keyup', (event) => {
      this.keys.delete(event.code);
    });

    window.addEventListener('blur', () => {
      this.keys.clear();
      this.mouseDown = false;
    });

    window.addEventListener('mouseup', () => {
      this.mouseDown = false;
    });

    canvas.addEventListener('pointerdown', (event) => {
      if (!this.locked) {
        this.requestPointerLock();
        return;
      }

      if (event.button === 0) this.mouseDown = true;
    });

    canvas.addEventListener('contextmenu', (event) => event.preventDefault());

    document.addEventListener('pointerlockchange', () => {
      this.locked = document.pointerLockElement === canvas;
      if (!this.locked) this.mouseDown = false;
      for (const listener of this.lockListeners) listener(this.locked);
    });

    document.addEventListener('mousemove', (event) => {
      if (this.locked) this.mouseDeltaX += event.movementX;
    });

    document.addEventListener('pointerlockerror', () => {
      this.locked = false;
      for (const listener of this.lockListeners) listener(false);
    });
  }

  public get isLocked(): boolean {
    return this.locked;
  }

  public isDown(code: string): boolean {
    return this.keys.has(code);
  }

  public consumePress(code: string): boolean {
    const wasPressed = this.pressed.has(code);
    this.pressed.delete(code);
    return wasPressed;
  }

  public consumeMouseDeltaX(): number {
    const delta = this.mouseDeltaX;
    this.mouseDeltaX = 0;
    return delta;
  }

  public get isFiring(): boolean {
    return this.locked && this.mouseDown;
  }

  public requestPointerLock(): void {
    void this.canvas.requestPointerLock();
  }

  public onLockChange(listener: (locked: boolean) => void): () => void {
    this.lockListeners.add(listener);
    return () => this.lockListeners.delete(listener);
  }

  public endFrame(): void {
    this.pressed.clear();
  }
}