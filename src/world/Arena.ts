import * as THREE from 'three';
import { createArmorMaterial, createRegolithMaterial, createRockMaterial } from '../rendering/SurfaceMaterials';

type Random = () => number;

function randomFrom(seed: number): Random {
  let state = seed >>> 0;
  return () => {
    state = (state * 1664525 + 1013904223) >>> 0;
    return state / 4294967296;
  };
}

function makePlanetTexture(): THREE.CanvasTexture {
  const canvas = document.createElement('canvas');
  canvas.width = 1024;
  canvas.height = 512;
  const context = canvas.getContext('2d');
  if (!context) throw new Error('Planet texture generation is unavailable.');

  const random = randomFrom(1846);
  context.fillStyle = '#173f72';
  context.fillRect(0, 0, canvas.width, canvas.height);

  for (let i = 0; i < 260; i++) {
    const x = random() * canvas.width;
    const y = random() * canvas.height;
    const width = 22 + random() * 150;
    const height = 4 + random() * 22;
    context.fillStyle = random() > 0.5 ? 'rgba(147,190,218,0.16)' : 'rgba(7,26,55,0.25)';
    context.beginPath();
    context.ellipse(x, y, width, height, (random() - 0.5) * 0.15, 0, Math.PI * 2);
    context.fill();
  }

  for (let i = 0; i < 1500; i++) {
    const x = random() * canvas.width;
    const y = random() * canvas.height;
    const radius = 1 + random() * 3.8;
    context.fillStyle = 'rgba(189,218,239,' + (0.025 + random() * 0.1) + ')';
    context.beginPath();
    context.ellipse(x, y, radius * 2.6, radius, 0, 0, Math.PI * 2);
    context.fill();
  }

  const texture = new THREE.CanvasTexture(canvas);
  texture.colorSpace = THREE.SRGBColorSpace;
  texture.anisotropy = 8;
  return texture;
}

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

function makeRockGeometry(seed: number): THREE.IcosahedronGeometry {
  const geometry = new THREE.IcosahedronGeometry(1, 2);
  const position = geometry.attributes.position;
  const random = randomFrom(seed);
  for (let i = 0; i < position.count; i++) {
    const factor = 0.72 + random() * 0.48;
    position.setXYZ(
      i,
      position.getX(i) * factor,
      position.getY(i) * factor,
      position.getZ(i) * factor
    );
  }
  geometry.computeVertexNormals();
  return geometry;
}

export class Arena {
  public readonly root = new THREE.Group();
  private readonly movingShips: Array<{ group: THREE.Group; speed: number; phase: number }> = [];

  constructor(scene: THREE.Scene) {
    scene.add(this.root);
    this.createSky(scene);
    this.createTerrain();
    this.createRockField();
    this.createCliffField();
    this.createGroundDebris();
    this.createOutpost();
    this.createPlanetaryBackdrop(scene);
    this.createShips(scene);
  }

  public getHeight(x: number, z: number): number {
    return Math.sin(x * 0.052 + z * 0.021) * 1.05
      + Math.cos(z * 0.047 - x * 0.016) * 0.85
      + Math.sin((x + z) * 0.105) * 0.22
      + Math.sin(x * 0.018) * Math.cos(z * 0.022) * 0.8;
  }

  public update(dt: number, time: number): void {
    for (const ship of this.movingShips) {
      ship.group.position.x += ship.speed * dt;
      ship.group.position.y += Math.sin(time * 0.32 + ship.phase) * dt * 0.65;
      if (ship.group.position.x > 125) ship.group.position.x = -125;
      if (ship.group.position.x < -125) ship.group.position.x = 125;
    }
  }

  private createSky(scene: THREE.Scene): void {
    const sky = new THREE.Mesh(
      new THREE.SphereGeometry(480, 48, 24),
      new THREE.ShaderMaterial({
        side: THREE.BackSide,
        depthWrite: false,
        uniforms: {
          topColor: { value: new THREE.Color(0x080f21) },
          horizonColor: { value: new THREE.Color(0x53647e) },
          lowerColor: { value: new THREE.Color(0x15192b) },
        },
        vertexShader: 'varying float vHeight; void main(){ vec4 worldPosition=modelMatrix*vec4(position,1.0); vHeight=normalize(worldPosition.xyz-cameraPosition).y; gl_Position=projectionMatrix*viewMatrix*worldPosition; }',
        fragmentShader: 'uniform vec3 topColor; uniform vec3 horizonColor; uniform vec3 lowerColor; varying float vHeight; void main(){ float h=smoothstep(-0.18,0.72,vHeight); vec3 c=mix(lowerColor,horizonColor,smoothstep(-0.28,0.05,vHeight)); c=mix(c,topColor,h); gl_FragColor=vec4(c,1.0); }',
      })
    );
    sky.renderOrder = -100;
    scene.add(sky);

    const random = randomFrom(71);
    const positions = new Float32Array(2200 * 3);
    const sizes = new Float32Array(2200);
    for (let i = 0; i < 2200; i++) {
      const theta = random() * Math.PI * 2;
      const phi = Math.acos(2 * random() - 1);
      const radius = 430;
      positions[i * 3] = radius * Math.sin(phi) * Math.cos(theta);
      positions[i * 3 + 1] = radius * Math.cos(phi);
      positions[i * 3 + 2] = radius * Math.sin(phi) * Math.sin(theta);
      sizes[i] = 0.4 + random() * 1.4;
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('size', new THREE.BufferAttribute(sizes, 1));
    const stars = new THREE.Points(
      geometry,
      new THREE.PointsMaterial({ color: 0xc9e6ff, size: 0.75, transparent: true, opacity: 0.8, sizeAttenuation: false })
    );
    stars.renderOrder = -90;
    scene.add(stars);
  }

  private createTerrain(): void {
    const geometry = new THREE.PlaneGeometry(260, 260, 220, 220);
    geometry.rotateX(-Math.PI / 2);
    const positions = geometry.attributes.position;
    for (let i = 0; i < positions.count; i++) {
      const x = positions.getX(i);
      const z = positions.getZ(i);
      positions.setY(i, this.getHeight(x, z) - 0.35);
    }
    geometry.computeVertexNormals();

    const ground = new THREE.Mesh(geometry, createRegolithMaterial());
    ground.receiveShadow = true;
    this.root.add(ground);

    const scorched = new THREE.MeshStandardMaterial({ color: 0x202630, roughness: 1 });
    const ridge = addBox(this.root, scorched, 0, -3.8, -128, 260, 7, 12);
    ridge.receiveShadow = true;
  }

  private createRockField(): void {
    const random = randomFrom(443);
    const geometry = makeRockGeometry(390);
    const materials = [
      createRockMaterial(0x68717c),
      createRockMaterial(0x78818b),
      createRockMaterial(0x39434e),
    ];

    const rocks = new THREE.InstancedMesh(geometry, materials[0], 280);
    rocks.instanceMatrix.setUsage(THREE.StaticDrawUsage);
    const dummy = new THREE.Object3D();
    for (let i = 0; i < 280; i++) {
      let x = (random() - 0.5) * 218;
      let z = (random() - 0.5) * 218;
      if (Math.hypot(x, z - 10) < 10) x += x < 0 ? -12 : 12;
      if (Math.hypot(x + 27, z + 20) < 24) x += 30;
      const scale = 0.22 + random() * 1.18;
      dummy.position.set(x, this.getHeight(x, z) - 0.25 + scale * 0.22, z);
      dummy.rotation.set(random() * 0.6, random() * Math.PI, random() * 0.6);
      dummy.scale.set(scale * (0.8 + random() * 0.6), scale * (0.55 + random() * 0.55), scale * (0.8 + random() * 0.55));
      dummy.updateMatrix();
      rocks.setMatrixAt(i, dummy.matrix);
      const tint = new THREE.Color().setHSL(0.58 + random() * 0.05, 0.1 + random() * 0.16, 0.17 + random() * 0.22);
      rocks.setColorAt(i, tint);
    }
    rocks.castShadow = true;
    rocks.receiveShadow = true;
    this.root.add(rocks);

    const foreground = new THREE.InstancedMesh(makeRockGeometry(929), materials[1], 72);
    for (let i = 0; i < 72; i++) {
      const side = random() > 0.5 ? 1 : -1;
      const x = side * (11 + random() * 17);
      const z = 5 + random() * 38;
      const scale = 0.5 + random() * 1.9;
      dummy.position.set(x, this.getHeight(x, z) - 0.1 + scale * 0.3, z);
      dummy.rotation.set(random() * 0.5, random() * 6, random() * 0.5);
      dummy.scale.set(scale, scale * (0.65 + random() * 0.4), scale * 0.8);
      dummy.updateMatrix();
      foreground.setMatrixAt(i, dummy.matrix);
    }
    foreground.castShadow = true;
    foreground.receiveShadow = true;
    this.root.add(foreground);
  }

  private createCliffField(): void {
    const random = randomFrom(1443);
    const geometry = makeRockGeometry(777);
    const material = createRockMaterial(0x8795a2);
    const cliffs = new THREE.InstancedMesh(geometry, material, 66);
    const dummy = new THREE.Object3D();

    for (let i = 0; i < 66; i++) {
      const side = i < 34 ? -1 : 1;
      const x = side < 0
        ? -43 - random() * 59
        : 42 + random() * 58;
      const z = -94 + random() * 142;
      const height = 5.5 + Math.pow(random(), 0.65) * 15;
      const width = height * (0.34 + random() * 0.5);
      dummy.position.set(x, this.getHeight(x, z) + height + 0.05, z);
      dummy.rotation.set((random() - 0.5) * 0.42, random() * Math.PI * 2, (random() - 0.5) * 0.35);
      dummy.scale.set(width, height, width * (0.55 + random() * 0.5));
      dummy.updateMatrix();
      cliffs.setMatrixAt(i, dummy.matrix);
      const tint = new THREE.Color().setHSL(0.58 + random() * 0.055, 0.1 + random() * 0.1, 0.2 + random() * 0.1);
      cliffs.setColorAt(i, tint);
    }

    cliffs.castShadow = true;
    cliffs.receiveShadow = true;
    cliffs.instanceMatrix.needsUpdate = true;
    this.root.add(cliffs);
  }

  private createGroundDebris(): void {
    const random = randomFrom(2941);
    const geometry = new THREE.IcosahedronGeometry(1, 1);
    const material = createRockMaterial(0x818b94);
    const debris = new THREE.InstancedMesh(geometry, material, 190);
    const dummy = new THREE.Object3D();

    for (let i = 0; i < 190; i++) {
      const side = random() > 0.5 ? 1 : -1;
      const x = side * (7.8 + random() * 24);
      const z = -3 + random() * 51;
      const size = 0.14 + Math.pow(random(), 2) * 1.18;
      dummy.position.set(x, this.getHeight(x, z) + size * 0.28 - 0.07, z);
      dummy.rotation.set(random() * 0.7, random() * Math.PI * 2, random() * 0.7);
      dummy.scale.set(size * (0.6 + random() * 0.9), size * (0.22 + random() * 0.42), size * (0.55 + random() * 0.8));
      dummy.updateMatrix();
      debris.setMatrixAt(i, dummy.matrix);
      const tint = new THREE.Color().setHSL(0.56 + random() * 0.07, 0.08 + random() * 0.13, 0.24 + random() * 0.12);
      debris.setColorAt(i, tint);
    }

    debris.castShadow = true;
    debris.receiveShadow = true;
    debris.instanceMatrix.needsUpdate = true;
    this.root.add(debris);

    const craterGeometry = new THREE.TorusGeometry(1, 0.055, 7, 36);
    const craterMaterial = new THREE.MeshStandardMaterial({
      color: 0x1b2028,
      roughness: 0.99,
      metalness: 0,
    });
    const craters = new THREE.InstancedMesh(craterGeometry, craterMaterial, 9);
    for (let i = 0; i < 9; i++) {
      const angle = random() * Math.PI * 2;
      const radius = 9 + random() * 28;
      const x = Math.cos(angle) * radius;
      const z = 7 + Math.sin(angle) * radius;
      const size = 0.9 + random() * 2.1;
      dummy.position.set(x, this.getHeight(x, z) - 0.16, z);
      dummy.rotation.set(Math.PI / 2 + (random() - 0.5) * 0.15, 0, 0);
      dummy.scale.set(size, size * (0.7 + random() * 0.3), size);
      dummy.updateMatrix();
      craters.setMatrixAt(i, dummy.matrix);
    }
    this.root.add(craters);
  }

  private createOutpost(): void {
    const base = new THREE.Group();
    base.position.set(-27, this.getHeight(-27, -22) + 0.1, -22);
    this.root.add(base);

    const armor = createArmorMaterial(0x52616e);
    const brightArmor = createArmorMaterial(0x889aa7);
    const dark = new THREE.MeshStandardMaterial({ color: 0x101923, metalness: 0.56, roughness: 0.65 });
    const red = new THREE.MeshStandardMaterial({ color: 0x842f37, metalness: 0.48, roughness: 0.62 });
    const glass = new THREE.MeshStandardMaterial({ color: 0x83d9ef, emissive: 0x1c9ecf, emissiveIntensity: 1.9, metalness: 0.34, roughness: 0.22 });
    const warm = new THREE.MeshStandardMaterial({ color: 0xffcc80, emissive: 0xff8d3c, emissiveIntensity: 2.6 });
    const concrete = new THREE.MeshStandardMaterial({ color: 0x454c55, roughness: 0.95 });

    addBox(base, concrete, 0, 0, 0, 34, 0.75, 27);
    addBox(base, armor, 0, 0.65, -7.5, 33, 1, 0.9);
    addBox(base, armor, -16, 1.1, 0, 0.9, 2.2, 25);
    addBox(base, armor, 16, 1.1, 0, 0.9, 2.2, 25);
    addBox(base, brightArmor, 0, 1.6, -13.25, 34, 0.34, 0.7);

    for (let i = -2; i <= 2; i++) {
      const x = i * 5.8;
      addBox(base, dark, x, 0.75, 5.4, 4.8, 1.5, 7.5);
      addBox(base, red, x, 1.54, 5.4, 4.8, 0.08, 7.5);
      addBox(base, glass, x, 1.65, 1.62, 3.35, 0.13, 0.07);
      addBox(base, armor, x, 2.2, 8.4, 4.1, 1.2, 1.6);
    }

    addBox(base, armor, -7, 3.6, -6.5, 12, 5.9, 8.2);
    addBox(base, brightArmor, -7, 6.55, -6.5, 12.3, 0.28, 8.5);
    addBox(base, dark, -7, 3.75, -2.34, 8.7, 3.4, 0.07);
    addBox(base, glass, -7, 4.2, -2.24, 6.2, 1.3, 0.12);
    addBox(base, red, -7, 6.15, -6.5, 3, 0.28, 8.2);

    addBox(base, dark, 8, 2.2, -8, 10, 3.2, 8);
    addBox(base, armor, 8, 3.9, -8, 10.5, 0.32, 8.5);
    for (let x = 4; x <= 12; x += 2) addBox(base, brightArmor, x, 2.7, -3.95, 0.14, 1.7, 0.12);
    addBox(base, glass, 8, 2.6, -3.87, 6.4, 0.18, 0.1);

    for (const side of [-1, 1]) {
      const tower = new THREE.Group();
      tower.position.set(side * 14.2, 0, -10);
      base.add(tower);
      addBox(tower, dark, 0, 3.6, 0, 3.1, 7.2, 3.1);
      addBox(tower, armor, 0, 7.3, 0, 4.2, 0.5, 4.2);
      addBox(tower, glass, 0, 5.2, -1.58, 1.9, 0.45, 0.1);
      addBox(tower, glass, 0, 5.2, 1.58, 1.9, 0.45, 0.1);
      addBox(tower, brightArmor, 0, 8.1, 0, 0.35, 1.4, 0.35);
      const beacon = new THREE.PointLight(0x51cfff, 8, 13, 2);
      beacon.position.set(0, 7.7, 0);
      tower.add(beacon);
    }

    for (let i = -2; i <= 2; i++) {
      const lamp = new THREE.PointLight(0xffa65e, 2.1, 8, 2);
      lamp.position.set(i * 5.8, 2.1, 2.0);
      base.add(lamp);
      addBox(base, warm, i * 5.8, 2.5, 2, 0.24, 0.14, 0.2);
    }

    // Layered utility hardware breaks up the large flat building surfaces.
    for (let i = 0; i < 8; i++) {
      const x = -15.5 + i * 4.4;
      addBox(base, dark, x, 2.5, -13.7, 1.6, 1.5, 0.55);
      addBox(base, brightArmor, x, 3.24, -13.42, 1.25, 0.08, 0.08);
      for (let slot = 0; slot < 4; slot++) {
        addBox(base, brightArmor, x - 0.48 + slot * 0.32, 2.52, -13.38, 0.075, 0.78, 0.08);
      }
    }

    // Exterior pressure conduits, routed around the forward wall and generator bay.
    const conduitMaterial = new THREE.MeshStandardMaterial({ color: 0x778895, metalness: 0.84, roughness: 0.34 });
    const conduitPaths = [
      [new THREE.Vector3(-15, 2.2, -5.5), new THREE.Vector3(-15, 4.6, -5.5), new THREE.Vector3(-12, 4.6, -5.5), new THREE.Vector3(-12, 5.9, -5.5)],
      [new THREE.Vector3(12, 1.9, -10), new THREE.Vector3(13, 1.9, -10), new THREE.Vector3(13, 4.5, -10), new THREE.Vector3(11, 4.5, -10)],
      [new THREE.Vector3(-5, 0.7, -12), new THREE.Vector3(-5, 1.8, -12), new THREE.Vector3(0, 1.8, -12), new THREE.Vector3(0, 2.5, -12)],
    ];
    for (const points of conduitPaths) {
      const curve = new THREE.CatmullRomCurve3(points);
      const pipe = new THREE.Mesh(new THREE.TubeGeometry(curve, 28, 0.095, 8, false), conduitMaterial);
      pipe.castShadow = true;
      pipe.receiveShadow = true;
      base.add(pipe);
    }

    // A raised radar dish and communications mast give the silhouette scale.
    const dish = new THREE.Mesh(
      new THREE.SphereGeometry(1.7, 22, 14, 0, Math.PI * 2, 0, Math.PI * 0.48),
      brightArmor
    );
    dish.position.set(11.8, 8.6, -8.7);
    dish.rotation.x = Math.PI;
    dish.castShadow = true;
    base.add(dish);
    addBox(base, dark, 11.8, 7.65, -8.7, 0.24, 2.0, 0.24);
    const redBeacon = new THREE.PointLight(0xff554c, 4.2, 15, 2);
    redBeacon.position.set(-10, 10.3, -8);
    base.add(redBeacon);

    const antenna = new THREE.Group();
    antenna.position.set(-10, 7, -8);
    base.add(antenna);
    addBox(antenna, brightArmor, 0, 2.1, 0, 0.24, 4.2, 0.24);
    const ring = new THREE.Mesh(new THREE.TorusGeometry(1.2, 0.06, 8, 32), glass);
    ring.position.y = 4.2;
    ring.rotation.x = Math.PI / 2;
    antenna.add(ring);

    // A line of blast walls and cargo modules gives the perimeter a readable silhouette.
    for (let i = 0; i < 5; i++) {
      const wall = new THREE.Group();
      wall.position.set(13 + i * 5.4, 0, -35 - (i % 2) * 2);
      wall.rotation.y = (i - 2) * 0.025;
      this.root.add(wall);
      addBox(wall, concrete, 0, 0.9, 0, 5.2, 1.8, 1.1);
      addBox(wall, armor, 0, 1.9, 0, 5.2, 0.18, 1.15);
      addBox(wall, red, 0, 1.25, 0.58, 3.1, 0.12, 0.08);
    }
  }

  private createPlanetaryBackdrop(scene: THREE.Scene): void {
    const planet = new THREE.Mesh(
      new THREE.SphereGeometry(48, 128, 80),
      new THREE.MeshStandardMaterial({
        map: makePlanetTexture(),
        color: 0xc3d8ea,
        roughness: 0.88,
        metalness: 0.015,
        emissive: 0x142c49,
        emissiveIntensity: 0.04,
      })
    );
    planet.position.set(68, 54, -182);
    planet.rotation.z = -0.2;
    scene.add(planet);

    const atmosphere = new THREE.Mesh(
      new THREE.SphereGeometry(49.5, 96, 64),
      new THREE.ShaderMaterial({
        transparent: true,
        side: THREE.BackSide,
        blending: THREE.NormalBlending,
        depthWrite: false,
        uniforms: { glowColor: { value: new THREE.Color(0x6ca9f2) } },
        vertexShader: 'varying vec3 vNormal; varying vec3 vView; void main(){ vec4 world=modelMatrix*vec4(position,1.0); vNormal=normalize(mat3(modelMatrix)*normal); vView=normalize(cameraPosition-world.xyz); gl_Position=projectionMatrix*viewMatrix*world; }',
        fragmentShader: 'uniform vec3 glowColor; varying vec3 vNormal; varying vec3 vView; void main(){ float rim=pow(1.0-max(dot(normalize(vNormal),normalize(vView)),0.0),2.3); gl_FragColor=vec4(glowColor, rim*0.16); }',
      })
    );
    atmosphere.position.copy(planet.position);
    scene.add(atmosphere);

    const moon = new THREE.Mesh(
      new THREE.SphereGeometry(4.3, 32, 24),
      new THREE.MeshStandardMaterial({ color: 0x8d8d99, roughness: 1, emissive: 0x252a38, emissiveIntensity: 0.28 })
    );
    moon.position.set(45, 30, -151);
    scene.add(moon);
  }

  private createShips(scene: THREE.Scene): void {
    const hull = new THREE.MeshStandardMaterial({ color: 0x273545, metalness: 0.84, roughness: 0.34 });
    const panel = new THREE.MeshStandardMaterial({ color: 0x617181, metalness: 0.9, roughness: 0.28 });
    const engine = new THREE.MeshStandardMaterial({ color: 0x65dfff, emissive: 0x169fe5, emissiveIntensity: 4.5 });
    const createShip = (x: number, y: number, z: number, scale: number, speed: number, phase: number) => {
      const ship = new THREE.Group();
      ship.position.set(x, y, z);
      ship.scale.setScalar(scale);
      scene.add(ship);
      addBox(ship, hull, 0, 0, 0, 15, 1.2, 3);
      addBox(ship, panel, -1.7, 0.78, 0, 6.5, 0.45, 2.6);
      addBox(ship, hull, 7.2, -0.1, 0, 3.2, 0.8, 2.3);
      addBox(ship, hull, -2, -0.15, 3.3, 9, 0.34, 5.3);
      addBox(ship, hull, -2, -0.15, -3.3, 9, 0.34, 5.3);
      addBox(ship, panel, -5.8, 0.8, 0, 3.1, 0.4, 1.4);
      for (const side of [-1, 1]) {
        const glow = addBox(ship, engine, -8, 0, side * 1.08, 0.28, 0.65, 0.55);
        glow.castShadow = false;
        const light = new THREE.PointLight(0x35cfff, 14, 22, 2);
        light.position.set(-8.6, 0, side * 1.08);
        ship.add(light);
      }
      this.movingShips.push({ group: ship, speed, phase });
    };

    createShip(-44, 28, -72, 1.05, 1.15, 0.2);
    createShip(24, 18, -92, 0.55, -0.5, 2.4);
  }
}