import * as THREE from 'three';
import { Input } from './Input';

function addBox(
  parent: THREE.Object3D,
  material: THREE.Material,
  x: number,
  y: number,
  z: number,
  width: number,
  height: number,
  depth: number
): THREE.Mesh {
  const mesh = new THREE.Mesh(new THREE.BoxGeometry(width, height, depth), material);
  mesh.position.set(x, y, z);
  mesh.castShadow = true;
  mesh.receiveShadow = true;
  parent.add(mesh);
  return mesh;
}

export class Player {
  public readonly group = new THREE.Group();
  public readonly position = new THREE.Vector3(0, 0, 9);
  public yaw = 0;
  public health = 100;
  public shield = 75;

  private readonly model = new THREE.Group();
  private readonly leftLeg = new THREE.Group();
  private readonly rightLeg = new THREE.Group();
  private readonly leftArm = new THREE.Group();
  private readonly rightArm = new THREE.Group();
  private readonly cameraOffset = new THREE.Vector3(0, 3.25, 7.6);
  private readonly forward = new THREE.Vector3();
  private readonly move = new THREE.Vector3();
  private animationTime = 0;
  private hurtCooldown = 0;

  constructor(scene: THREE.Scene) {
    this.group.position.copy(this.position);
    this.group.add(this.model);
    scene.add(this.group);
    this.createMarine();
  }

  public update(dt: number, input: Input, groundHeight: (x: number, z: number) => number): void {
    this.hurtCooldown = Math.max(0, this.hurtCooldown - dt);

    if (input.isLocked) {
      this.yaw -= input.consumeMouseDeltaX() * 0.00235;
      const forwardInput = Number(input.isDown('KeyW')) - Number(input.isDown('KeyS'));
      const sideInput = Number(input.isDown('KeyD')) - Number(input.isDown('KeyA'));

      this.forward.set(-Math.sin(this.yaw), 0, -Math.cos(this.yaw));
      const right = new THREE.Vector3(Math.cos(this.yaw), 0, -Math.sin(this.yaw));
      this.move.copy(this.forward).multiplyScalar(forwardInput).addScaledVector(right, sideInput);
      const moving = this.move.lengthSq() > 0.001;
      if (moving) this.move.normalize();

      const sprinting = input.isDown('ShiftLeft') || input.isDown('ShiftRight');
      const speed = sprinting ? 8.6 : 5.6;
      this.position.addScaledVector(this.move, speed * dt);
      this.position.x = THREE.MathUtils.clamp(this.position.x, -104, 104);
      this.position.z = THREE.MathUtils.clamp(this.position.z, -104, 104);

      if (moving) this.animationTime += dt * (sprinting ? 12.2 : 9.0);
      else this.animationTime += dt * 2.2;

      const gait = moving ? Math.sin(this.animationTime) : Math.sin(this.animationTime) * 0.12;
      this.leftLeg.rotation.x = gait * (sprinting ? 0.68 : 0.48);
      this.rightLeg.rotation.x = -gait * (sprinting ? 0.68 : 0.48);
      this.model.position.y = moving ? Math.abs(gait) * 0.045 : Math.sin(this.animationTime) * 0.012;
      this.group.rotation.y = this.yaw;
      this.model.rotation.y = 0;
    }

    this.position.y = groundHeight(this.position.x, this.position.z);
    this.group.position.copy(this.position);
  }

  public updateCamera(camera: THREE.PerspectiveCamera, dt: number): void {
    const desiredOffset = this.cameraOffset.clone().applyAxisAngle(new THREE.Vector3(0, 1, 0), this.yaw);
    const desiredPosition = this.position.clone().add(desiredOffset);
    camera.position.lerp(desiredPosition, 1 - Math.exp(-dt * 5.5));

    this.forward.set(-Math.sin(this.yaw), 0, -Math.cos(this.yaw));
    const target = this.position.clone().addScaledVector(this.forward, 3.4);
    target.y += 1.72;
    camera.lookAt(target);
  }

  public getMuzzlePosition(): THREE.Vector3 {
    this.group.updateMatrixWorld(true);
    return this.group.localToWorld(new THREE.Vector3(0, 1.6, -2.0));
  }

  public takeDamage(amount: number): boolean {
    if (this.hurtCooldown > 0 || this.health <= 0) return false;
    this.hurtCooldown = 0.35;

    let remaining = amount;
    const absorbed = Math.min(this.shield, remaining);
    this.shield -= absorbed;
    remaining -= absorbed;
    this.health = Math.max(0, this.health - remaining);
    return true;
  }

  public restore(): void {
    this.health = 100;
    this.shield = 75;
    this.hurtCooldown = 0;
  }

  private createMarine(): void {
    const undersuit = new THREE.MeshStandardMaterial({ color: 0x111a23, roughness: 0.78, metalness: 0.32 });
    const armor = new THREE.MeshStandardMaterial({ color: 0x9aa6ae, roughness: 0.52, metalness: 0.58 });
    const darkArmor = new THREE.MeshStandardMaterial({ color: 0x26313d, roughness: 0.62, metalness: 0.64 });
    const red = new THREE.MeshStandardMaterial({ color: 0x732c35, roughness: 0.58, metalness: 0.52 });
    const visor = new THREE.MeshPhysicalMaterial({ color: 0x101c29, roughness: 0.16, metalness: 0.82, clearcoat: 1, emissive: 0x06263a, emissiveIntensity: 0.6 });
    const cyan = new THREE.MeshStandardMaterial({ color: 0x5edcff, emissive: 0x119acb, emissiveIntensity: 2.6, metalness: 0.36, roughness: 0.24 });
    const rifle = new THREE.MeshStandardMaterial({ color: 0x202b36, roughness: 0.4, metalness: 0.78 });
    const barrel = new THREE.MeshStandardMaterial({ color: 0x9baab3, roughness: 0.27, metalness: 0.92 });

    addBox(this.model, undersuit, 0, 1.18, 0, 0.72, 0.78, 0.42);
    addBox(this.model, armor, 0, 1.42, -0.06, 0.82, 0.48, 0.48);
    addBox(this.model, darkArmor, 0, 0.94, 0.02, 0.72, 0.2, 0.45);
    addBox(this.model, red, -0.39, 1.4, -0.03, 0.13, 0.44, 0.5);
    addBox(this.model, darkArmor, 0, 1.1, 0.245, 0.35, 0.24, 0.035);
    addBox(this.model, cyan, 0.27, 1.43, -0.314, 0.07, 0.27, 0.035);
    addBox(this.model, darkArmor, 0, 0.78, 0.01, 0.62, 0.32, 0.4);

    const helmet = new THREE.Mesh(new THREE.SphereGeometry(0.39, 24, 18), armor);
    helmet.scale.set(1, 1.02, 0.95);
    helmet.position.set(0, 1.94, -0.02);
    helmet.castShadow = true;
    this.model.add(helmet);

    const face = new THREE.Mesh(new THREE.SphereGeometry(0.295, 24, 16), visor);
    face.scale.set(1, 0.62, 0.44);
    face.position.set(0, 1.91, -0.315);
    this.model.add(face);
    addBox(this.model, darkArmor, 0, 1.68, -0.31, 0.35, 0.12, 0.13);
    addBox(this.model, cyan, -0.34, 1.96, -0.015, 0.07, 0.16, 0.17);
    addBox(this.model, cyan, 0.34, 1.96, -0.015, 0.07, 0.16, 0.17);
    addBox(this.model, darkArmor, 0, 1.78, 0.31, 0.57, 0.58, 0.35);
    addBox(this.model, armor, 0, 1.83, 0.48, 0.48, 0.36, 0.08);
    addBox(this.model, cyan, 0, 2.07, 0.525, 0.22, 0.08, 0.035);

    for (const side of [-1, 1]) {
      const shoulder = new THREE.Mesh(new THREE.SphereGeometry(0.27, 16, 12), side < 0 ? red : armor);
      shoulder.scale.set(1.18, 0.9, 1.05);
      shoulder.position.set(side * 0.52, 1.52, -0.01);
      shoulder.castShadow = true;
      this.model.add(shoulder);
      addBox(this.model, darkArmor, side * 0.53, 1.23, -0.02, 0.24, 0.3, 0.3);
    }

    this.leftLeg.position.set(-0.23, 0.78, 0);
    this.rightLeg.position.set(0.23, 0.78, 0);
    this.model.add(this.leftLeg, this.rightLeg);
    for (const leg of [this.leftLeg, this.rightLeg]) {
      const thigh = new THREE.Mesh(new THREE.CylinderGeometry(0.16, 0.19, 0.55, 10), undersuit);
      thigh.position.y = -0.3;
      thigh.castShadow = true;
      leg.add(thigh);
      const plate = new THREE.Mesh(new THREE.BoxGeometry(0.35, 0.33, 0.12), armor);
      plate.position.set(0, -0.28, -0.11);
      plate.castShadow = true;
      leg.add(plate);
      const shin = new THREE.Mesh(new THREE.CylinderGeometry(0.12, 0.16, 0.42, 10), darkArmor);
      shin.position.set(0, -0.68, -0.015);
      shin.castShadow = true;
      leg.add(shin);
      addBox(leg, armor, 0, -0.89, -0.14, 0.3, 0.17, 0.48);
      addBox(leg, cyan, 0, -0.64, -0.14, 0.16, 0.045, 0.045);
    }

    this.leftArm.position.set(-0.49, 1.42, -0.04);
    this.rightArm.position.set(0.49, 1.42, -0.04);
    this.model.add(this.leftArm, this.rightArm);
    for (const [arm, side] of [[this.leftArm, -1], [this.rightArm, 1]] as const) {
      const upper = new THREE.Mesh(new THREE.CylinderGeometry(0.12, 0.16, 0.53, 10), undersuit);
      upper.rotation.x = -0.85;
      upper.position.set(side * 0.035, -0.16, -0.18);
      upper.castShadow = true;
      arm.add(upper);
      const forearm = new THREE.Mesh(new THREE.CylinderGeometry(0.11, 0.15, 0.48, 10), armor);
      forearm.rotation.x = -1.0;
      forearm.position.set(side * 0.04, -0.28, -0.49);
      forearm.castShadow = true;
      arm.add(forearm);
      const glove = new THREE.Mesh(new THREE.SphereGeometry(0.13, 12, 10), darkArmor);
      glove.position.set(side * 0.04, -0.41, -0.7);
      arm.add(glove);
    }

    const weapon = new THREE.Group();
    weapon.position.set(0, 1.31, -0.63);
    this.model.add(weapon);
    addBox(weapon, rifle, 0, 0, -0.37, 0.24, 0.24, 0.9);
    addBox(weapon, darkArmor, 0, 0.12, -0.32, 0.19, 0.08, 0.78);
    addBox(weapon, barrel, 0, 0.025, -0.93, 0.095, 0.095, 0.56);
    addBox(weapon, darkArmor, 0, -0.17, -0.28, 0.13, 0.28, 0.18);
    addBox(weapon, darkArmor, 0, 0.23, -0.28, 0.08, 0.11, 0.26);
    addBox(weapon, cyan, 0.126, 0.01, -0.4, 0.025, 0.09, 0.28);
    const muzzle = new THREE.Mesh(new THREE.CylinderGeometry(0.075, 0.075, 0.15, 12), barrel);
    muzzle.rotation.x = Math.PI / 2;
    muzzle.position.z = -1.24;
    weapon.add(muzzle);
  }
}