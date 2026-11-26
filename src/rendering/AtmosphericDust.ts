import * as THREE from 'three';

type ParticleSeed = {
  x: number;
  y: number;
  z: number;
  size: number;
  phase: number;
  speed: number;
};

/** Cheap, depth-aware floating grit to keep the air from looking sterile. */
export class AtmosphericDust {
  private readonly points: THREE.Points;
  private readonly material: THREE.ShaderMaterial;

  constructor(scene: THREE.Scene, seed = 714) {
    let state = seed >>> 0;
    const random = (): number => {
      state = (state * 1664525 + 1013904223) >>> 0;
      return state / 4294967296;
    };

    const count = 520;
    const particles: ParticleSeed[] = [];
    for (let i = 0; i < count; i++) {
      particles.push({
        x: (random() - 0.5) * 150,
        y: -1 + random() * 20,
        z: -105 + random() * 155,
        size: 1.2 + random() * 3.8,
        phase: random() * 18,
        speed: 0.08 + random() * 0.32,
      });
    }

    const geometry = new THREE.BufferGeometry();
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(particles.flatMap(p => [p.x, p.y, p.z]), 3));
    geometry.setAttribute('aSize', new THREE.Float32BufferAttribute(particles.map(p => p.size), 1));
    geometry.setAttribute('aPhase', new THREE.Float32BufferAttribute(particles.map(p => p.phase), 1));
    geometry.setAttribute('aSpeed', new THREE.Float32BufferAttribute(particles.map(p => p.speed), 1));

    this.material = new THREE.ShaderMaterial({
      transparent: true,
      depthWrite: false,
      uniforms: {
        uTime: { value: 0 },
        uColor: { value: new THREE.Color(0x9db9cc) },
        uOpacity: { value: 0.19 },
      },
      vertexShader: `
        uniform float uTime;
        attribute float aSize;
        attribute float aPhase;
        attribute float aSpeed;
        varying float vFade;
        void main() {
          vec3 p = position;
          p.y = mod(p.y + uTime * aSpeed + aPhase, 19.0) - 1.0;
          p.x += sin(uTime * 0.17 + aPhase) * 0.55;
          p.z += cos(uTime * 0.11 + aPhase) * 0.35;
          vec4 viewPosition = modelViewMatrix * vec4(p, 1.0);
          float depth = max(1.0, -viewPosition.z);
          gl_PointSize = clamp(aSize * (65.0 / depth), 0.7, 6.5);
          vFade = 1.0 - smoothstep(8.0, 125.0, depth);
          gl_Position = projectionMatrix * viewPosition;
        }
      `,
      fragmentShader: `
        uniform vec3 uColor;
        uniform float uOpacity;
        varying float vFade;
        void main() {
          float radius = length(gl_PointCoord - vec2(0.5));
          float soft = 1.0 - smoothstep(0.12, 0.5, radius);
          float alpha = soft * uOpacity * vFade;
          gl_FragColor = vec4(uColor, alpha);
          #include <tonemapping_fragment>
          #include <colorspace_fragment>
        }
      `,
    });

    this.points = new THREE.Points(geometry, this.material);
    this.points.frustumCulled = false;
    this.points.renderOrder = 2;
    scene.add(this.points);
  }

  public update(elapsedSeconds: number): void {
    this.material.uniforms.uTime.value = elapsedSeconds;
  }
}
