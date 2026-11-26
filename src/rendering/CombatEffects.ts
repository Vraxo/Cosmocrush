import * as THREE from 'three';

interface TransientLine {
  object: THREE.Line;
  life: number;
}

export class CombatEffects {
  private readonly lines: TransientLine[] = [];
  private readonly flashes: Array<{ light: THREE.PointLight; life: number }> = [];

  constructor(private readonly scene: THREE.Scene) {}

  public fire(start: THREE.Vector3, end: THREE.Vector3): void {
    const geometry = new THREE.BufferGeometry().setFromPoints([start, end]);
    const material = new THREE.LineBasicMaterial({ color: 0x8ce9ff, transparent: true, opacity: 0.96 });
    const line = new THREE.Line(geometry, material);
    this.scene.add(line);
    this.lines.push({ object: line, life: 0.075 });

    const flash = new THREE.PointLight(0x55cfff, 16, 7, 2);
    flash.position.copy(start);
    this.scene.add(flash);
    this.flashes.push({ light: flash, life: 0.055 });
  }

  public impact(position: THREE.Vector3, color = 0xff9b5e): void {
    const geometry = new THREE.SphereGeometry(0.1, 8, 6);
    const material = new THREE.MeshBasicMaterial({ color, transparent: true, opacity: 1 });
    const spark = new THREE.Mesh(geometry, material);
    spark.position.copy(position);
    this.scene.add(spark);
    this.lines.push({ object: new THREE.Line(
      new THREE.BufferGeometry().setFromPoints([position, position.clone().add(new THREE.Vector3(0, 0.36, 0))]),
      new THREE.LineBasicMaterial({ color, transparent: true, opacity: 0.85 })
    ), life: 0.12 });
    this.flashes.push({ light: new THREE.PointLight(color, 4.5, 3.5, 2), life: 0.075 });
    this.flashes[this.flashes.length - 1].light.position.copy(position);
    this.scene.add(this.flashes[this.flashes.length - 1].light);
    // Tiny impact meshes are short-lived and do not become part of the world state.
    spark.userData.life = 0.1;
    spark.userData.transient = true;
    this.scene.add(spark);
  }

  public update(dt: number): void {
    for (let i = this.lines.length - 1; i >= 0; i--) {
      const item = this.lines[i];
      item.life -= dt;
      if (item.life <= 0) {
        this.scene.remove(item.object);
        item.object.geometry.dispose();
        (item.object.material as THREE.Material).dispose();
        this.lines.splice(i, 1);
      }
    }

    for (let i = this.flashes.length - 1; i >= 0; i--) {
      const item = this.flashes[i];
      item.life -= dt;
      item.light.intensity = Math.max(0, item.light.intensity * 0.5);
      if (item.life <= 0) {
        this.scene.remove(item.light);
        this.flashes.splice(i, 1);
      }
    }

    for (const object of this.scene.children) {
      if (object.userData.transient) {
        object.userData.life -= dt;
        if (object.userData.life <= 0) {
          this.scene.remove(object);
          if (object instanceof THREE.Mesh) {
            object.geometry.dispose();
            (object.material as THREE.Material).dispose();
          }
        }
      }
    }
  }
}