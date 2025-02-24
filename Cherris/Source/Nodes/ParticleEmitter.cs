using Raylib_cs;

namespace Cherris;

public partial class ParticleEmitter : Node2D
{
    public bool OneShot { get; set; } = false;
    public int Amount { get; set; } = 50;
    public Vector2 Gravity { get; set; } = new(980, 0);
    public Vector2 SpawnAreaMin { get; set; } = Vector2.Zero;
    public Vector2 SpawnAreaMax { get; set; } = Vector2.Zero;
    public Vector2 StartVelocityMin { get; set; } = new(100, 0);
    public Vector2 StartVelocityMax { get; set; } = new(200, 0);
    public float StartScaleMin { get; set; } = 1.0f;
    public float StartScaleMax { get; set; } = 1.0f;
    public float EndScaleMin { get; set; } = 0.0f;
    public float EndScaleMax { get; set; } = 0.0f;
    public float Spread { get; set; } = 45.0f;
    public float Explosiveness { get; set; } = 0.0f;
    public float Lifetime { get; set; } = 1f;
    public Color Color { get; set; } = Color.White;

    public bool Emitting
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            if (!(field) || !OneShot)
            {
                return;
            }

            emitted = false;
            cycleTimer = 0;
        }
    } = true;

    private readonly List<Particle> particles = [];
    private readonly Random random = new();
    private float cycleTimer;
    private bool emitted;

    public override void Process()
    {
        base.Process();
        ProcessParticles();

        if (Emitting && !(OneShot && emitted))
        {
            cycleTimer += TimeServer.Delta;
            GenerateParticles();
        }
    }

    public override void Draw()
    {
        base.Draw();
        SubmitDrawCalls();
    }

    private void ProcessParticles()
    {
        float delta = TimeServer.Delta;

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            Particle particle = particles[i];
            particle.Process(delta);

            if (particle.RemainingLifetime <= 0)
            {
                particles.RemoveAt(i);
            }
        }
    }

    private void GenerateParticles()
    {
        if (Explosiveness >= 0.999f)
        {
            // Burst emission - create all particles at once
            int particlesToCreate = Amount - particles.Count;
            for (int i = 0; i < particlesToCreate; i++)
            {
                particles.Add(CreateParticle());
            }
            emitted = true;
            cycleTimer = 0;
        }
        else
        {
            float cycleDuration = Lifetime * (1 - Explosiveness);
            if (cycleDuration <= 0 || Amount <= 0) return;

            float emissionInterval = cycleDuration / Amount;
            int particlesToEmit = (int)(cycleTimer / emissionInterval);

            // Cap the emission to remaining amount
            particlesToEmit = (int)float.Min(particlesToEmit, Amount - particles.Count);

            for (int i = 0; i < particlesToEmit; i++)
            {
                particles.Add(CreateParticle());
                cycleTimer -= emissionInterval;
            }

            if (particles.Count >= Amount)
            {
                emitted = true;
                cycleTimer = 0;
            }
        }

        if (OneShot && emitted)
        {
            Emitting = false;
        }
    }

    private Particle CreateParticle()
    {
        Vector2 randomPosition = new(
            NextFloat(SpawnAreaMin.X, SpawnAreaMax.X),
            NextFloat(SpawnAreaMin.Y, SpawnAreaMax.Y)
        );

        Vector2 initialVelocity = new(
            NextFloat(StartVelocityMin.X, StartVelocityMax.X),
            NextFloat(StartVelocityMin.Y, StartVelocityMax.Y)
        );

        ApplySpread(ref initialVelocity);

        float startScale = NextFloat(StartScaleMin, StartScaleMax);
        float endScale = NextFloat(EndScaleMin, EndScaleMax);

        return new Particle()
        {
            Position = randomPosition,
            StartScale = startScale,
            EndScale = endScale,
            Color = Color,
            Velocity = initialVelocity,
            Acceleration = Gravity,
            RemainingLifetime = Lifetime,
            InitialLifetime = Lifetime
        };
    }

    private void ApplySpread(ref Vector2 velocity)
    {
        if (Spread <= 0)
        {
            return;
        }
        
        float speed = velocity.Length();

        if (speed <= 0)
        {
            return;
        }

        var originalAngle = float.Atan2(velocity.Y, velocity.X);
        float spreadRad = MathF.PI * Spread / 180f;
        float angleOffset = NextFloat(-spreadRad, spreadRad);

        float newAngle = originalAngle + angleOffset;
        velocity.X = float.Cos(newAngle) * speed;
        velocity.Y = float.Sin(newAngle) * speed;
    }

    private void SubmitDrawCalls()
    {
        foreach (Particle particle in particles)
        {
            RenderServer.Instance.Submit(() =>
            {
                Raylib.DrawRectangleV(
                    GlobalPosition + particle.Position,
                    new Vector2(particle.CurrentScale, particle.CurrentScale),
                    particle.Color);
            }, Layer);
        }
    }

    private float NextFloat(float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }

    private class Particle
    {
        public Vector2 Position { get; set; }
        public float StartScale { get; set; }
        public float EndScale { get; set; }
        public float CurrentScale { get; private set; }
        public Color Color { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public float RemainingLifetime { get; set; }
        public float InitialLifetime { get; set; }

        public void Process(float delta)
        {
            RemainingLifetime -= delta;
            Velocity += Acceleration * delta;
            Position += Velocity * delta;

            if (InitialLifetime > 0)
            {
                float t = 1 - (RemainingLifetime / InitialLifetime);
                CurrentScale = StartScale + (EndScale - StartScale) * t;
            }
            else
            {
                CurrentScale = EndScale;
            }
        }
    }
}