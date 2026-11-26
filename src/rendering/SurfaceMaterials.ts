import * as THREE from 'three';

interface SurfaceMaps {
  color: THREE.CanvasTexture;
  normal: THREE.CanvasTexture;
  roughness: THREE.CanvasTexture;
}

type Random = () => number;

function seededRandom(seed: number): Random {
  let state = seed >>> 0;
  return () => {
    state = (state * 1664525 + 1013904223) >>> 0;
    return state / 4294967296;
  };
}

function smooth(t: number): number {
  return t * t * (3 - 2 * t);
}

function hash(x: number, y: number, seed: number): number {
  let n = Math.imul(x, 374761393) + Math.imul(y, 668265263) + Math.imul(seed, 1442695041);
  n = Math.imul(n ^ (n >>> 13), 1274126177);
  return ((n ^ (n >>> 16)) >>> 0) / 4294967295;
}

function periodicNoise(u: number, v: number, cells: number, seed: number): number {
  const x = u * cells;
  const y = v * cells;
  const ix = Math.floor(x);
  const iy = Math.floor(y);
  const fx = smooth(x - ix);
  const fy = smooth(y - iy);
  const wrap = (value: number): number => ((value % cells) + cells) % cells;
  const a = hash(wrap(ix), wrap(iy), seed);
  const b = hash(wrap(ix + 1), wrap(iy), seed);
  const c = hash(wrap(ix), wrap(iy + 1), seed);
  const d = hash(wrap(ix + 1), wrap(iy + 1), seed);
  const top = THREE.MathUtils.lerp(a, b, fx);
  const bottom = THREE.MathUtils.lerp(c, d, fx);
  return THREE.MathUtils.lerp(top, bottom, fy);
}

function fractal(u: number, v: number, seed: number): number {
  return (
    periodicNoise(u, v, 4, seed) * 0.43
    + periodicNoise(u, v, 9, seed + 11) * 0.27
    + periodicNoise(u, v, 19, seed + 29) * 0.17
    + periodicNoise(u, v, 41, seed + 47) * 0.09
    + periodicNoise(u, v, 83, seed + 71) * 0.04
  );
}

function makeTexture(canvas: HTMLCanvasElement, colorSpace: THREE.ColorSpace): THREE.CanvasTexture {
  const texture = new THREE.CanvasTexture(canvas);
  texture.colorSpace = colorSpace;
  texture.wrapS = THREE.RepeatWrapping;
  texture.wrapT = THREE.RepeatWrapping;
  texture.anisotropy = 8;
  texture.generateMipmaps = true;
  texture.minFilter = THREE.LinearMipmapLinearFilter;
  texture.magFilter = THREE.LinearFilter;
  return texture;
}

function buildRegolithMaps(size = 512): SurfaceMaps {
  const colorCanvas = document.createElement('canvas');
  const normalCanvas = document.createElement('canvas');
  const roughnessCanvas = document.createElement('canvas');
  colorCanvas.width = colorCanvas.height = size;
  normalCanvas.width = normalCanvas.height = size;
  roughnessCanvas.width = roughnessCanvas.height = size;

  const colorContext = colorCanvas.getContext('2d');
  const normalContext = normalCanvas.getContext('2d');
  const roughnessContext = roughnessCanvas.getContext('2d');
  if (!colorContext || !normalContext || !roughnessContext) {
    throw new Error('Procedural surface textures are unavailable.');
  }

  const color = colorContext.createImageData(size, size);
  const normal = normalContext.createImageData(size, size);
  const roughness = roughnessContext.createImageData(size, size);
  const heights = new Float32Array(size * size);
  const random = seededRandom(18471);

  for (let y = 0; y < size; y++) {
    for (let x = 0; x < size; x++) {
      const u = x / size;
      const v = y / size;
      const broad = fractal(u, v, 91);
      const fine = periodicNoise(u, v, 143, 191);
      const grit = random() - 0.5;
      const ridges = Math.pow(Math.max(0, Math.sin((u * 9.2 + v * 4.1 + broad * 0.8) * Math.PI * 2)), 18);
      const height = broad * 0.6 + fine * 0.28 + ridges * 0.12 + grit * 0.055;
      heights[y * size + x] = height;

      const i = (y * size + x) * 4;
      const mineral = periodicNoise(u, v, 13, 421);
      const shadowPocket = Math.max(0, 0.48 - height) * 23;
      const base = 37 + broad * 25 + fine * 12 + grit * 13 - shadowPocket;
      const warmSediment = Math.max(0, mineral - 0.52) * 21;
      color.data[i] = THREE.MathUtils.clamp(base + 3 + warmSediment, 12, 112);
      color.data[i + 1] = THREE.MathUtils.clamp(base + 7 + warmSediment * 0.58, 14, 110);
      color.data[i + 2] = THREE.MathUtils.clamp(base + 13 + fine * 8, 18, 120);
      color.data[i + 3] = 255;

      const rough = Math.round(178 + broad * 57 + grit * 28);
      roughness.data[i] = rough;
      roughness.data[i + 1] = rough;
      roughness.data[i + 2] = rough;
      roughness.data[i + 3] = 255;
    }
  }

  for (let y = 0; y < size; y++) {
    for (let x = 0; x < size; x++) {
      const left = heights[y * size + (x + size - 1) % size];
      const right = heights[y * size + (x + 1) % size];
      const up = heights[((y + size - 1) % size) * size + x];
      const down = heights[((y + 1) % size) * size + x];
      const dx = (right - left) * 2.7;
      const dy = (down - up) * 2.7;
      const length = Math.sqrt(dx * dx + dy * dy + 1);
      const i = (y * size + x) * 4;
      normal.data[i] = Math.round(((-dx / length) * 0.5 + 0.5) * 255);
      normal.data[i + 1] = Math.round(((-dy / length) * 0.5 + 0.5) * 255);
      normal.data[i + 2] = Math.round((1 / length * 0.5 + 0.5) * 255);
      normal.data[i + 3] = 255;
    }
  }

  colorContext.putImageData(color, 0, 0);
  normalContext.putImageData(normal, 0, 0);
  roughnessContext.putImageData(roughness, 0, 0);

  return {
    color: makeTexture(colorCanvas, THREE.SRGBColorSpace),
    normal: makeTexture(normalCanvas, THREE.NoColorSpace),
    roughness: makeTexture(roughnessCanvas, THREE.NoColorSpace),
  };
}

function buildArmorMaps(size = 256): SurfaceMaps {
  const colorCanvas = document.createElement('canvas');
  const normalCanvas = document.createElement('canvas');
  const roughnessCanvas = document.createElement('canvas');
  colorCanvas.width = colorCanvas.height = size;
  normalCanvas.width = normalCanvas.height = size;
  roughnessCanvas.width = roughnessCanvas.height = size;

  const colorContext = colorCanvas.getContext('2d');
  const normalContext = normalCanvas.getContext('2d');
  const roughnessContext = roughnessCanvas.getContext('2d');
  if (!colorContext || !normalContext || !roughnessContext) {
    throw new Error('Procedural armor textures are unavailable.');
  }

  const random = seededRandom(773);
  const color = colorContext.createImageData(size, size);
  const normal = normalContext.createImageData(size, size);
  const roughness = roughnessContext.createImageData(size, size);
  const heights = new Float32Array(size * size);
  const scratches: Array<{ x: number; y: number; length: number; shade: number }> = [];
  const scratchMask = new Float32Array(size * size);

  for (let i = 0; i < 380; i++) {
    scratches.push({
      x: random() * size,
      y: random() * size,
      length: 3 + random() * 43,
      shade: random(),
    });
  }

  // Rasterize the sparse scratches once instead of checking all 380 marks
  // for every texture pixel.
  for (const scratch of scratches) {
    const value = scratch.shade > 0.55 ? 0.2 : -0.14;
    const startX = Math.max(0, Math.floor(scratch.x - scratch.length));
    const endX = Math.min(size - 1, Math.ceil(scratch.x + scratch.length));
    const centerY = Math.round(scratch.y);
    for (let x = startX; x <= endX; x++) {
      for (let dy = -1; dy <= 1; dy++) {
        const y = centerY + dy;
        if (y < 0 || y >= size) continue;
        const strength = (1 - Math.abs(y - scratch.y) / 1.1) * value;
        const index = y * size + x;
        if (Math.abs(y - scratch.y) < 1.1 && Math.abs(x - scratch.x) < scratch.length) {
          if (Math.abs(strength) > Math.abs(scratchMask[index])) scratchMask[index] = strength;
        }
      }
    }
  }
  for (let y = 0; y < size; y++) {
    for (let x = 0; x < size; x++) {
      const i = (y * size + x) * 4;
      const brush = (random() - 0.5) * 0.09;
      const damage = scratchMask[y * size + x];
      const grain = periodicNoise(x / size, y / size, 61, 107) - 0.5;
      const height = grain * 0.18 + damage * 0.45 + brush;
      heights[y * size + x] = height;
      const value = 112 + brush * 100 + damage * 70 + grain * 17;
      color.data[i] = THREE.MathUtils.clamp(value + 5, 55, 185);
      color.data[i + 1] = THREE.MathUtils.clamp(value + 10, 60, 190);
      color.data[i + 2] = THREE.MathUtils.clamp(value + 16, 65, 198);
      color.data[i + 3] = 255;
      const rough = Math.round(132 + random() * 66 - Math.abs(damage) * 65);
      roughness.data[i] = rough;
      roughness.data[i + 1] = rough;
      roughness.data[i + 2] = rough;
      roughness.data[i + 3] = 255;
    }
  }

  for (let y = 0; y < size; y++) {
    for (let x = 0; x < size; x++) {
      const left = heights[y * size + (x + size - 1) % size];
      const right = heights[y * size + (x + 1) % size];
      const up = heights[((y + size - 1) % size) * size + x];
      const down = heights[((y + 1) % size) * size + x];
      const dx = (right - left) * 3.5;
      const dy = (down - up) * 3.5;
      const length = Math.sqrt(dx * dx + dy * dy + 1);
      const i = (y * size + x) * 4;
      normal.data[i] = Math.round(((-dx / length) * 0.5 + 0.5) * 255);
      normal.data[i + 1] = Math.round(((-dy / length) * 0.5 + 0.5) * 255);
      normal.data[i + 2] = Math.round((1 / length * 0.5 + 0.5) * 255);
      normal.data[i + 3] = 255;
    }
  }

  colorContext.putImageData(color, 0, 0);
  normalContext.putImageData(normal, 0, 0);
  roughnessContext.putImageData(roughness, 0, 0);

  return {
    color: makeTexture(colorCanvas, THREE.SRGBColorSpace),
    normal: makeTexture(normalCanvas, THREE.NoColorSpace),
    roughness: makeTexture(roughnessCanvas, THREE.NoColorSpace),
  };
}

let regolithMaps: SurfaceMaps | null = null;
let armorMaps: SurfaceMaps | null = null;

export function createRegolithMaterial(): THREE.MeshStandardMaterial {
  regolithMaps ??= buildRegolithMaps();
  regolithMaps.color.repeat.set(8, 8);
  regolithMaps.normal.repeat.set(8, 8);
  regolithMaps.roughness.repeat.set(8, 8);
  return new THREE.MeshStandardMaterial({
    map: regolithMaps.color,
    normalMap: regolithMaps.normal,
    roughnessMap: regolithMaps.roughness,
    color: 0xc6d0d8,
    roughness: 0.94,
    metalness: 0.015,
    normalScale: new THREE.Vector2(1.45, 1.45),
  });
}

export function createRockMaterial(color = 0x626c78): THREE.MeshStandardMaterial {
  regolithMaps ??= buildRegolithMaps();
  return new THREE.MeshStandardMaterial({
    map: regolithMaps.color,
    normalMap: regolithMaps.normal,
    roughnessMap: regolithMaps.roughness,
    color,
    roughness: 0.96,
    metalness: 0.025,
    normalScale: new THREE.Vector2(1.1, 1.1),
  });
}

export function createArmorMaterial(color = 0x485664): THREE.MeshStandardMaterial {
  armorMaps ??= buildArmorMaps();
  return new THREE.MeshStandardMaterial({
    map: armorMaps.color,
    normalMap: armorMaps.normal,
    roughnessMap: armorMaps.roughness,
    color,
    roughness: 0.46,
    metalness: 0.74,
    normalScale: new THREE.Vector2(0.7, 0.7),
  });
}
