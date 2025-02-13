using Raylib_cs;

namespace Cherris;

public partial class ParticleGenerator : Node2D
{
    public Vector2 ParticleSize { get; set; } = new(1,1);
    public float Lifetime { get; set; } = 10;
    public Vector2 SpawnAreaMin { get; set; } = Vector2.Zero;
    public Vector2 SpawnAreaMax { get; set; } = new(0, 100);
    public float MinAcceleration { get; set; } = 0;
    public float MaxAcceleration { get; set; } = 300;
    public int Amount { get; set; } = 1;
    public Color ParticleColor { get; set; } = Color.White;

    private readonly List<Particle> particles = [];
    private readonly Random random = new();

    public override void Update()
    {
        base.Update();
        ProcessParticles();
        GenerateParticles();
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
        for (int i = 0; i < Amount; i++)
        {
            particles.Add(CreateParticle());
        }
    }

    private void SubmitDrawCalls()
    {
        foreach (Particle particle in particles)
        {
            RenderServer.Instance.Submit(() =>
            {
                Raylib.DrawRectangleV(
                    GlobalPosition + particle.Position,
                    particle.Size,
                    particle.Color);
            }, Layer);
        }
    }

    private Particle CreateParticle()
    {
        Vector2 randomPosition = new(
            NextFloat(SpawnAreaMin.X, SpawnAreaMax.X),
            NextFloat(SpawnAreaMin.Y, SpawnAreaMax.Y)
        );

        float randomAcceleration = NextFloat(MinAcceleration, MaxAcceleration);

        return new()
        {
            Position = randomPosition,
            Size = ParticleSize,
            Color = ParticleColor,
            Speed = 100,
            Acceleration = randomAcceleration,
            RemainingLifetime = Lifetime
        };
    }

    private float NextFloat(float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }

    private class Particle
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Color Color { get; set; }
        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float RemainingLifetime { get; set; }

        public void Process(float delta)
        {
            RemainingLifetime -= delta;
            Speed += Acceleration * delta;
            Position += new Vector2(Speed * delta, 0);
        }
    }
}