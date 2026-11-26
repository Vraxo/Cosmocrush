import * as THREE from 'three';

export class AlienEnemy {
  public readonly group = new THREE.Group();
  public readonly hitSphere = new THREE.Sphere(new THREE.Vector3(), 1.35);
  public readonly id: number;
  public readonly position = new THREE.Vector3();
  public health = 90;
  public alive = true;
  public attackTimer = 0.8;
  public age = 0;
  public deathTimer = 0;
  private readonly limbs: THREE.Group[] = [];
  private readonly bodyMaterial: THREE.MeshStandardMaterial;
  private readonly eyeMaterial: THREE.MeshStandardMaterial;
  private readonly shellMaterial: THREE.MeshPhysicalMaterial;
  private readonly size: number;

  constructor(scene: THREE.Scene, id: number, x: number, y: number, z: number, variant: number) {
    this.id = id;
    this.position.set(x, y, z);
    this.size = variant === 2 ? 1.18 : variant === 1 ? 0.9 : 1;
    const bodyColor = variant === 1 ? 0x392b36 : variant === 2 ? 0x342b40 : 0x3e3035;
    const shellColor = variant === 1 ? 0x75404d : variant === 2 ? 0x513a6b : 0x693e43;
    this.bodyMaterial = new THREE.MeshStandardMaterial({ color: bodyColor, roughness: 0.78, metalness: 0.08 });
    this.shellMaterial = new THREE.MeshPhysicalMaterial({
      color: shellColor,
      roughness: 0.33,
      metalness: 0.18,
      clearcoat: 0.65,
      clearcoatRoughness: 0.36,
    });
    this.eyeMaterial = new THREE.MeshStandardMaterial({
      color: 0xff554e,
      emissive: 0xe32638,
      emissiveIntensity: 3.8,
      roughness: 0.2,
    });

    scene.add(this.group);
    this.group.position.copy(this.position);
    this.buildBody(variant);
  }

  public update(dt: number, playerPosition: THREE.Vector3, groundHeight: (x: number, z: number) => number, onAttack: () => void): void {
    this.age += dt;
    if (!this.alive) {
      this.deathTimer -= dt;
      this.group.rotation.z += dt * 2.8;
      this.group.position.y -= dt * 1.8;
      this.group.scale.multiplyScalar(Math.max(0.9, 1 - dt * 0.8));
      return;
    }

    const dx = playerPosition.x - this.position.x;
    const dz = playerPosition.z - this.position.z;
    const distance = Math.max(0.001, Math.hypot(dx, dz));
    this.group.rotation.y = Math.atan2(-dx, -dz);

    if (distance > 1.95) {
      const speed = this.size > 1.1 ? 2.1 : 2.7;
      this.position.x += dx / distance * speed * dt;
      this.position.z += dz / distance * speed * dt;
    } else {
      this.attackTimer -= dt;
      if (this.attackTimer <= 0) {
        this.attackTimer = 1.25 + Math.random() * 0.45;
        onAttack();
      }
    }

    this.position.y = groundHeight(this.position.x, this.position.z);
    this.group.position.copy(this.position);
    const stride = Math.sin(this.age * 8.8) * (distance > 2 ? 0.52 : 0.1);
    for (let i = 0; i < this.limbs.length; i++) {
      this.limbs[i].rotation.x = stride * (i % 2 === 0 ? 1 : -1);
      this.limbs[i].rotation.z = (i === 0 || i === 2 ? -1 : 1) * 0.08;
    }
    this.group.position.y += Math.sin(this.age * 5.2) * 0.035;
    this.hitSphere.center.set(this.position.x, this.position.y + 1.05 * this.size, this.position.z);
    this.hitSphere.radius = 1.25 * this.size;
  }

  public damage(amount: number): boolean {
    if (!this.alive) return false;
    this.health -= amount;
    this.shellMaterial.emissive.setHex(0x501016);
    this.shellMaterial.emissiveIntensity = 1.1;
    if (this.health <= 0) {
      this.alive = false;
      this.deathTimer = 0.42;
      this.group.scale.setScalar(this.size);
      return true;
    }
    this.group.scale.setScalar(this.size * 1.035);
    return false;
  }

  public dispose(scene: THREE.Scene): void {
    scene.remove(this.group);
    this.group.traverse((object) => {
      if (object instanceof THREE.Mesh) {
        object.geometry.dispose();
        if (Array.isArray(object.material)) {
          for (const material of object.material) material.dispose();
        } else {
          object.material.dispose();
        }
      }
    });
  }

  private buildBody(variant: number): void {
    this.group.scale.setScalar(this.size);
    const body = new THREE.Group();
    this.group.add(body);

    const abdomen = new THREE.Mesh(new THREE.SphereGeometry(0.72, 24, 18), this.bodyMaterial);
    abdomen.scale.set(0.92, 0.68, 1.12);
    abdomen.position.set(0, 0.95, 0.03);
    abdomen.castShadow = true;
    body.add(abdomen);

    const carapace = new THREE.Mesh(new THREE.SphereGeometry(0.78, 24, 16), this.shellMaterial);
    carapace.scale.set(0.98, 0.46, 1.12);
    carapace.position.set(0, 1.35, 0.13);
    carapace.castShadow = true;
    body.add(carapace);

    const head = new THREE.Mesh(new THREE.SphereGeometry(0.4, 20, 16), this.shellMaterial);
    head.scale.set(0.88, 1.1, 0.78);
    head.position.set(0, 1.55, -0.64);
    head.castShadow = true;
    body.add(head);

    const jaw = new THREE.Mesh(new THREE.SphereGeometry(0.28, 16, 12), this.bodyMaterial);
    jaw.scale.set(1, 0.55, 1.1);
    jaw.position.set(0, 1.28, -0.88);
    body.add(jaw);

    for (const side of [-1, 1]) {
      const eye = new THREE.Mesh(new THREE.SphereGeometry(0.105, 12, 10), this.eyeMaterial);
      eye.position.set(side * 0.255, 1.64, -0.92);
      body.add(eye);

      const horn = new THREE.Mesh(new THREE.ConeGeometry(0.13, 0.68, 8), this.shellMaterial);
      horn.position.set(side * 0.36, 1.98, -0.5);
      horn.rotation.z = side * -0.65;
      horn.rotation.x = -0.22;
      horn.castShadow = true;
      body.add(horn);

      const shoulder = new THREE.Mesh(new THREE.SphereGeometry(0.26, 14, 12), this.shellMaterial);
      shoulder.scale.set(1.25, 0.9, 1.1);
      shoulder.position.set(side * 0.67, 1.24, -0.19);
      body.add(shoulder);
    }

    // Six articulated limbs create the insectoid silhouette; the paired rear legs support the body.
    for (let i = 0; i < 6; i++) {
      const side = i % 2 === 0 ? -1 : 1;
      const row = Math.floor(i / 2);
      const limb = new THREE.Group();
      limb.position.set(side * (0.57 + row * 0.03), 1.0, -0.48 + row * 0.47);
      body.add(limb);
      this.limbs.push(limb);

      const upper = new THREE.Mesh(new THREE.CylinderGeometry(0.09, 0.17, 0.72, 9), this.bodyMaterial);
      upper.position.set(side * 0.23, -0.37, 0);
      upper.rotation.z = side * -0.46;
      upper.castShadow = true;
      limb.add(upper);

      const joint = new THREE.Mesh(new THREE.SphereGeometry(0.13, 12, 10), this.shellMaterial);
      joint.position.set(side * 0.45, -0.7, 0);
      limb.add(joint);

      const lower = new THREE.Mesh(new THREE.CylinderGeometry(0.045, 0.11, 0.78, 8), this.shellMaterial);
      lower.position.set(side * 0.62, -1.02, -0.12);
      lower.rotation.z = side * 0.34;
      lower.rotation.x = -0.18;
      lower.castShadow = true;
      limb.add(lower);

      const claw = new THREE.Mesh(new THREE.ConeGeometry(0.075, 0.36, 7), this.shellMaterial);
      claw.position.set(side * 0.75, -1.39, -0.3);
      claw.rotation.z = side * 0.18;
      claw.rotation.x = -0.28;
      limb.add(claw);
    }

    for (let i = 0; i < 5; i++) {
      const spine = new THREE.Mesh(new THREE.ConeGeometry(0.095, 0.48, 7), this.shellMaterial);
      spine.position.set((i - 2) * 0.2, 1.75 + Math.sin(i * 0.8) * 0.06, 0.15);
      spine.rotation.x = 0.22;
      spine.castShadow = true;
      body.add(spine);
    }
  }
}