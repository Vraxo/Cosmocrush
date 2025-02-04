namespace Cherris;

public partial class ParticleGenerator : Node2D
{
    public Vector2 ParticleSize { get; set; } = Vector2.One;
    public float Lifetime { get; set; } = 10;
    public Vector2 SpawnAreaMin { get; set; } = Vector2.Zero;
    public Vector2 SpawnAreaMax { get; set; } = new(0, 100);
    public float MinAcceleration { get; set; } = 0;
    public float MaxAcceleration { get; set; } = 300;
    public int Amount { get; set; } = 1;

    private readonly List<Particle> particles = [];
    private readonly Random random = new();

    public override void Update()
    {
        base.Update();

        ProcessParticles();
        GenerateParticles();
    }

    protected override void Draw()
    {
        base.Draw();

        DrawRectangle(Position, Size, Color.Red);
    }

    private void Remove(Particle particle)
    {
        particles.Remove(particle);
    }

    private void ProcessParticles()
    {
        foreach (Particle particle in particles)
        {
            particle.Process();
        }
    }

    private void GenerateParticles()
    {
        for (int i = 0; i < Amount; i++)
        {
            particles.Add(GetParticle());
        }
    }

    private Particle GetParticle()
    {
        SpawnAreaMax = new(SpawnAreaMax.X, VisualServer.WindowSize.Y);

        Vector2 randomPosition = new(
            NextFloat(SpawnAreaMin.X, SpawnAreaMax.X),
            NextFloat(SpawnAreaMin.Y, SpawnAreaMax.Y)
        );

        float randomAcceleration = NextFloat(MinAcceleration, MaxAcceleration);

        return new()
        {
            Lifetime = Lifetime,
            Size = ParticleSize,
            Position = randomPosition,
            Acceleration = randomAcceleration
        };
    }

    private float NextFloat(float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }
}