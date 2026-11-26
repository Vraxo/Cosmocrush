import * as THREE from 'three';

function panel(
  scene: THREE.Scene,
  color: number,
  intensity: number,
  position: THREE.Vector3,
  rotation: THREE.Euler,
  width: number,
  height: number
): void {
  const material = new THREE.MeshBasicMaterial({
    color: new THREE.Color(color).multiplyScalar(intensity),
    side: THREE.DoubleSide,
  });
  const mesh = new THREE.Mesh(new THREE.PlaneGeometry(width, height), material);
  mesh.position.copy(position);
  mesh.rotation.copy(rotation);
  scene.add(mesh);
}

/** A compact studio/sky reflection probe for convincing PBR metal response. */
export function createEnvironmentMap(renderer: THREE.WebGLRenderer): THREE.WebGLRenderTarget {
  const scene = new THREE.Scene();
  scene.background = new THREE.Color(0x0b1727);

  const shell = new THREE.Mesh(
    new THREE.SphereGeometry(36, 48, 24),
    new THREE.MeshBasicMaterial({ color: 0x17273c, side: THREE.BackSide })
  );
  scene.add(shell);

  panel(scene, 0xb9d6f4, 1.1, new THREE.Vector3(0, 12, -3), new THREE.Euler(-Math.PI / 2.5, 0, 0), 18, 11);
  panel(scene, 0x8bc9f4, 0.7, new THREE.Vector3(-13, 2, -3), new THREE.Euler(0, Math.PI / 2, 0), 10, 13);
  panel(scene, 0x5a9ee7, 0.75, new THREE.Vector3(12, 1, -7), new THREE.Euler(0, -Math.PI / 2, 0), 8, 12);
  panel(scene, 0xffb278, 0.8, new THREE.Vector3(-5, -4, 12), new THREE.Euler(0.15, Math.PI, 0), 12, 5);
  panel(scene, 0xeff6ff, 0.6, new THREE.Vector3(0, -12, -2), new THREE.Euler(Math.PI / 2, 0, 0), 12, 8);

  const key = new THREE.PointLight(0xe9f3ff, 120, 80, 2);
  key.position.set(-11, 15, 13);
  scene.add(key);
  const fill = new THREE.PointLight(0x6baeff, 80, 65, 2);
  fill.position.set(13, 1, -11);
  scene.add(fill);

  const generator = new THREE.PMREMGenerator(renderer);
  generator.compileCubemapShader();
  const target = generator.fromScene(scene, 0.04, 0.1, 100);
  generator.dispose();

  scene.traverse((object) => {
    if (object instanceof THREE.Mesh) {
      object.geometry.dispose();
      const materials = Array.isArray(object.material) ? object.material : [object.material];
      for (const material of materials) material.dispose();
    }
  });

  return target;
}
