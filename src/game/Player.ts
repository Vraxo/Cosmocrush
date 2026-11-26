import * as THREE from 'three';
import { RoundedBoxGeometry } from 'three/addons/geometries/RoundedBoxGeometry.js';
import { Input } from './Input';

function addCapsule(
  parent: THREE.Object3D,
  material: THREE.Material,
  radius: number,
  length: number,
  x: number,
  y: number,
  z: number
): THREE.Mesh {
  const mesh = new THREE.Mesh(new THREE.CapsuleGeometry(radius, length, 5, 10), material);
  mesh.position.set(x, y, z);
  mesh.castShadow = true;
  mesh.receiveShadow = true;
  parent.add(mesh);
  return mesh;
}

function addRoundedBox(
  parent: THREE.Object3D,
  material: THREE.Material,
  x: number,
  y: number,
  z: number,
  width: number,
  height: number,
  depth: number,
  radius = 0.06
): THREE.Mesh {
  const mesh = new THREE.Mesh(
    new RoundedBoxGeometry(width, height, depth, 3, Math.min(radius, width / 3, height / 3, depth / 3)),
    material
  );
  mesh.position.set(x, y, z);
  mesh.castShadow = true;
  mesh.receiveShadow = true;
  parent.add(mesh);
  return mesh;
}

function addSphere(
  parent: THREE.Object3D,
  material: THREE.Material,
  x: number,
  y: number,
  z: number,
  scaleX: number,
  scaleY: number,
  scaleZ: number
): THREE.Mesh {
  const mesh = new THREE.Mesh(new THREE.SphereGeometry(1, 20, 14), material);
  mesh.position.set(x, y, z);
  mesh.scale.set(scaleX, scaleY, scaleZ);
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
  private readonly torso = new THREE.Group();
  private readonly head = new THREE.Group();
  private readonly leftLeg = new THREE.Group();
  private readonly rightLeg = new THREE.Group();
  private readonly leftKnee = new THREE.Group();
  private readonly rightKnee = new THREE.Group();
  private readonly leftArm = new THREE.Group();
  private readonly rightArm = new THREE.Group();
  private readonly leftElbow = new THREE.Group();
  private readonly rightElbow = new THREE.Group();
  private readonly weapon = new THREE.Group();
  private readonly forward = new THREE.Vector3();
  private readonly move = new THREE.Vector3();
  private readonly right = new THREE.Vector3();
  private readonly cameraOffset = new THREE.Vector3(-1.05, 2.65, 6.15);
  private animationTime = 0;
  private movementBlend = 0;
  private fireKick = 0;
  private hurtCooldown = 0;

  constructor(scene: THREE.Scene) {
    this.group.position.copy(this.position);
    this.group.add(this.model);
    scene.add(this.group);
    this.createMarine();
  }

  public update(dt: number, input: Input, groundHeight: (x: number, z: number) => number): void {
    this.hurtCooldown = Math.max(0, this.hurtCooldown - dt);

    let sprinting = false;
    if (input.isLocked) {
      this.yaw -= input.consumeMouseDeltaX() * 0.00235;
      const forwardInput = Number(input.isDown('KeyW')) - Number(input.isDown('KeyS'));
      const sideInput = Number(input.isDown('KeyD')) - Number(input.isDown('KeyA'));

      this.forward.set(-Math.sin(this.yaw), 0, -Math.cos(this.yaw));
      this.right.set(Math.cos(this.yaw), 0, -Math.sin(this.yaw));
      this.move.copy(this.forward).multiplyScalar(forwardInput).addScaledVector(this.right, sideInput);
      const moving = this.move.lengthSq() > 0.001;
      if (moving) this.move.normalize();

      sprinting = input.isDown('ShiftLeft') || input.isDown('ShiftRight');
      const speed = sprinting ? 8.6 : 5.6;
      this.position.addScaledVector(this.move, speed * dt);
      this.position.x = THREE.MathUtils.clamp(this.position.x, -104, 104);
      this.position.z = THREE.MathUtils.clamp(this.position.z, -104, 104);
    }

    this.position.y = groundHeight(this.position.x, this.position.z);
    this.group.position.copy(this.position);
    this.group.rotation.y = this.yaw;
    this.updateLocomotion(dt, sprinting);
  }

  public updateCamera(camera: THREE.PerspectiveCamera, dt: number): void {
    const desiredOffset = this.cameraOffset.clone().applyAxisAngle(new THREE.Vector3(0, 1, 0), this.yaw);
    const desiredPosition = this.position.clone().add(desiredOffset);
    camera.position.lerp(desiredPosition, 1 - Math.exp(-dt * 5.5));

    this.forward.set(-Math.sin(this.yaw), 0, -Math.cos(this.yaw));
    const target = this.position.clone().addScaledVector(this.forward, 15.5);
    target.y += 1.62;
    camera.lookAt(target);
  }

  public getMuzzlePosition(): THREE.Vector3 {
    this.group.updateMatrixWorld(true);
    return this.weapon.localToWorld(new THREE.Vector3(0, 0.02, -1.42));
  }

  public triggerFire(): void {
    this.fireKick = Math.min(0.42, this.fireKick + 0.28);
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

  private updateLocomotion(dt: number, sprinting: boolean): void {
    const moving = this.move.lengthSq() > 0.001;
    this.movementBlend = THREE.MathUtils.damp(this.movementBlend, moving ? 1 : 0, 8, dt);
    this.fireKick = Math.max(0, this.fireKick - dt * 3.8);

    const cadence = sprinting ? 12.8 : 8.6;
    this.animationTime += dt * (moving ? cadence : 1.8);
    const phase = this.animationTime;
    const gait = Math.sin(phase);
    const oppositeGait = Math.sin(phase + Math.PI);
    const stride = this.movementBlend * (sprinting ? 0.66 : 0.46);
    const breath = Math.sin(phase * 0.5) * 0.012 * (1 - this.movementBlend);

    this.model.position.y = Math.abs(Math.sin(phase * 2)) * 0.035 * this.movementBlend + breath;
    this.torso.position.y = 1.07 + Math.sin(phase * 2) * 0.018 * this.movementBlend + breath;
    this.torso.rotation.x = 0.035 * this.movementBlend + Math.sin(phase * 0.5) * 0.012;
    this.torso.rotation.z = Math.sin(phase) * 0.018 * this.movementBlend;

    const leftSwing = gait * stride;
    const rightSwing = oppositeGait * stride;
    this.leftLeg.rotation.x = leftSwing;
    this.rightLeg.rotation.x = rightSwing;
    this.leftKnee.rotation.x = Math.max(0, -leftSwing) * (sprinting ? 1.15 : 0.72);
    this.rightKnee.rotation.x = Math.max(0, -rightSwing) * (sprinting ? 1.15 : 0.72);
    this.leftKnee.rotation.z = 0;
    this.rightKnee.rotation.z = 0;
    this.leftArm.rotation.x = 0.96 - gait * 0.13 * this.movementBlend;
    this.rightArm.rotation.x = 0.96 + gait * 0.13 * this.movementBlend + this.fireKick;
    this.leftArm.rotation.z = -0.3 - gait * 0.035 * this.movementBlend;
    this.rightArm.rotation.z = 0.045 + gait * 0.035 * this.movementBlend;
    this.leftElbow.rotation.x = -0.16 - Math.max(0, gait) * 0.15 * this.movementBlend;
    this.rightElbow.rotation.x = -0.1;
    this.weapon.rotation.x = -this.fireKick * 0.7;
  }

  private createMarine(): void {
    const undersuit = new THREE.MeshStandardMaterial({ color: 0x111923, roughness: 0.78, metalness: 0.34 });
    const armor = new THREE.MeshStandardMaterial({ color: 0x8997a2, roughness: 0.38, metalness: 0.72 });
    const brightArmor = new THREE.MeshStandardMaterial({ color: 0xb9c4cb, roughness: 0.32, metalness: 0.78 });
    const darkArmor = new THREE.MeshStandardMaterial({ color: 0x202c38, roughness: 0.52, metalness: 0.66 });
    const red = new THREE.MeshStandardMaterial({ color: 0x73313a, roughness: 0.5, metalness: 0.56 });
    const visor = new THREE.MeshPhysicalMaterial({
      color: 0x101b2a,
      roughness: 0.12,
      metalness: 0.8,
      clearcoat: 1,
      emissive: 0x0b2637,
      emissiveIntensity: 0.45,
    });
    const cyan = new THREE.MeshStandardMaterial({
      color: 0x64d9f3,
      emissive: 0x0e789b,
      emissiveIntensity: 0.8,
      metalness: 0.42,
      roughness: 0.28,
    });
    const rifle = new THREE.MeshStandardMaterial({ color: 0x18232e, roughness: 0.38, metalness: 0.82 });
    const barrel = new THREE.MeshStandardMaterial({ color: 0x82929f, roughness: 0.28, metalness: 0.9 });

    this.model.add(this.torso, this.head, this.leftLeg, this.rightLeg, this.leftArm, this.rightArm);

    // Pelvis and layered torso armour.
    addRoundedBox(this.torso, darkArmor, 0, -0.03, 0.015, 0.68, 0.38, 0.43, 0.1);
    addRoundedBox(this.torso, undersuit, 0, 0.2, 0.015, 0.73, 0.7, 0.42, 0.12);
    addRoundedBox(this.torso, armor, 0, 0.32, -0.2, 0.82, 0.58, 0.22, 0.09);
    addRoundedBox(this.torso, brightArmor, 0, 0.43, -0.32, 0.58, 0.34, 0.08, 0.045);
    addRoundedBox(this.torso, darkArmor, 0, 0.02, -0.235, 0.46, 0.17, 0.08, 0.035);

    for (let i = 0; i < 3; i++) {
      addRoundedBox(this.torso, darkArmor, 0, 0.14 - i * 0.15, -0.245, 0.46 - i * 0.025, 0.055, 0.055, 0.02);
    }

    // Shoulder plates, chest insignia, and the compact rear power unit.
    for (const side of [-1, 1]) {
      const shoulder = addSphere(this.torso, side < 0 ? red : armor, side * 0.48, 0.43, -0.02, 0.31, 0.24, 0.31);
      shoulder.rotation.z = side * -0.16;
      addRoundedBox(this.torso, darkArmor, side * 0.48, 0.17, 0.02, 0.25, 0.28, 0.27, 0.07);
      addRoundedBox(this.torso, brightArmor, side * 0.48, 0.42, -0.235, 0.18, 0.08, 0.035, 0.02);
    }
    addRoundedBox(this.torso, darkArmor, 0, 0.31, 0.26, 0.56, 0.58, 0.28, 0.08);
    addRoundedBox(this.torso, armor, 0, 0.35, 0.42, 0.42, 0.46, 0.12, 0.055);
    addRoundedBox(this.torso, cyan, 0.24, 0.42, -0.28, 0.055, 0.2, 0.025, 0.012);
    addRoundedBox(this.torso, red, -0.25, 0.4, -0.29, 0.055, 0.21, 0.025, 0.012);

    const pack = new THREE.Group();
    pack.position.set(0, 0.3, 0.43);
    this.torso.add(pack);
    addRoundedBox(pack, darkArmor, 0, 0, 0.06, 0.55, 0.7, 0.28, 0.08);
    addRoundedBox(pack, armor, 0, 0.04, 0.22, 0.34, 0.42, 0.08, 0.04);
    for (const side of [-1, 1]) {
      addRoundedBox(pack, red, side * 0.3, 0.06, 0.04, 0.12, 0.43, 0.23, 0.035);
      const vent = addRoundedBox(pack, brightArmor, side * 0.19, 0.04, 0.27, 0.055, 0.25, 0.025, 0.01);
      vent.rotation.z = side * 0.08;
    }
    const packLight = new THREE.Mesh(new THREE.SphereGeometry(0.07, 16, 10), cyan);
    packLight.position.set(0, 0.38, 0.16);
    pack.add(packLight);

    // Helmet shell, recessed visor, cheek plates, and side comms hardware.
    const helmet = addSphere(this.head, armor, 0, 0, 0, 0.39, 0.43, 0.38);
    helmet.position.y = 1.92;
    addSphere(this.head, darkArmor, 0, 1.7, 0.015, 0.32, 0.15, 0.34);
    const visorMesh = addRoundedBox(this.head, visor, 0, 1.97, -0.315, 0.52, 0.2, 0.12, 0.075);
    visorMesh.rotation.x = -0.08;
    addRoundedBox(this.head, cyan, 0, 2.075, -0.36, 0.24, 0.025, 0.025, 0.01);
    for (const side of [-1, 1]) {
      addRoundedBox(this.head, brightArmor, side * 0.38, 1.91, -0.01, 0.12, 0.22, 0.27, 0.045);
      addRoundedBox(this.head, darkArmor, side * 0.4, 1.93, -0.2, 0.08, 0.1, 0.08, 0.025);
      addSphere(this.head, cyan, side * 0.4, 1.93, -0.22, 0.035, 0.035, 0.025);
    }
    addRoundedBox(this.head, brightArmor, 0, 2.33, 0.03, 0.13, 0.12, 0.16, 0.035);

    // Articulated legs with separate knee and ankle pivots.
    this.leftLeg.position.set(-0.23, 0.84, 0.015);
    this.rightLeg.position.set(0.23, 0.84, 0.015);
    for (const [leg, knee, side] of [
      [this.leftLeg, this.leftKnee, -1],
      [this.rightLeg, this.rightKnee, 1],
    ] as const) {
      addCapsule(leg, undersuit, 0.17, 0.39, 0, -0.25, 0);
      addRoundedBox(leg, armor, 0, -0.24, -0.13, 0.32, 0.36, 0.15, 0.055);
      addSphere(leg, darkArmor, 0, -0.53, -0.03, 0.18, 0.17, 0.17);
      knee.position.set(0, -0.56, 0);
      leg.add(knee);
      addCapsule(knee, darkArmor, 0.13, 0.35, 0, -0.22, 0.015);
      addRoundedBox(knee, brightArmor, 0, -0.22, -0.105, 0.26, 0.27, 0.095, 0.04);
      addRoundedBox(knee, armor, 0, -0.48, -0.16, 0.28, 0.14, 0.42, 0.045);
      addRoundedBox(knee, darkArmor, 0, -0.51, 0.02, 0.24, 0.11, 0.2, 0.035);
      addRoundedBox(knee, cyan, 0, -0.13, -0.16, 0.11, 0.035, 0.025, 0.01);
      addRoundedBox(leg, red, side * 0.14, -0.27, -0.18, 0.045, 0.13, 0.04, 0.01);
    }

    // Arms are raised in a continuous ready-to-fire stance, not swapped poses.
    this.leftArm.position.set(-0.5, 1.57, -0.015);
    this.rightArm.position.set(0.5, 1.57, -0.015);
    for (const [arm, elbow, side] of [
      [this.leftArm, this.leftElbow, -1],
      [this.rightArm, this.rightElbow, 1],
    ] as const) {
      addCapsule(arm, undersuit, 0.15, 0.36, side * 0.025, -0.23, 0.02);
      const shoulder = addRoundedBox(arm, side < 0 ? red : armor, side * 0.015, -0.05, 0.015, 0.34, 0.25, 0.34, 0.075);
      shoulder.rotation.z = side * -0.12;
      elbow.position.set(side * 0.04, -0.43, -0.04);
      arm.add(elbow);
      addCapsule(elbow, darkArmor, 0.125, 0.33, 0, -0.21, -0.03);
      addRoundedBox(elbow, armor, 0, -0.2, -0.135, 0.25, 0.3, 0.1, 0.04);
      addRoundedBox(elbow, brightArmor, 0, -0.1, -0.19, 0.15, 0.055, 0.025, 0.012);
      addSphere(elbow, darkArmor, 0, -0.43, -0.04, 0.14, 0.13, 0.14);
    }

    // Compact magnetic assault rifle with a clear barrel, receiver, optic and side rail.
    this.weapon.position.set(0.02, -0.48, -0.24);
    this.rightArm.add(this.weapon);
    addRoundedBox(this.weapon, rifle, 0, 0, -0.35, 0.23, 0.23, 0.82, 0.045);
    addRoundedBox(this.weapon, darkArmor, 0, 0.12, -0.31, 0.19, 0.09, 0.58, 0.025);
    addRoundedBox(this.weapon, armor, 0, -0.05, -0.7, 0.14, 0.08, 0.42, 0.02);
    addRoundedBox(this.weapon, darkArmor, 0, -0.17, -0.24, 0.13, 0.28, 0.18, 0.025);
    addRoundedBox(this.weapon, darkArmor, 0, 0.22, -0.24, 0.08, 0.13, 0.23, 0.02);
    addRoundedBox(this.weapon, cyan, 0.124, 0.02, -0.38, 0.025, 0.075, 0.25, 0.008);
    const muzzle = new THREE.Mesh(new THREE.CylinderGeometry(0.07, 0.07, 0.16, 14), barrel);
    muzzle.rotation.x = Math.PI / 2;
    muzzle.position.z = -0.83;
    this.weapon.add(muzzle);
    addRoundedBox(this.weapon, brightArmor, 0, 0.19, -0.23, 0.12, 0.06, 0.12, 0.02);
  }
}