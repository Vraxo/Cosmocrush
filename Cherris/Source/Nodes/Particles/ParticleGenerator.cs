namespace Cherris;

public class ParticleGenerator : Node2D
{
    public Vector2 ParticleSize { get; set; } = Vector2.One;
    public float Lifetime { get; set; } = 10;
    public Vector2 SpawnAreaMin { get; set; } = Vector2.Zero;
    public Vector2 SpawnAreaMax { get; set; } = new(0, 100);
    public float MinAcceleration { get; set; } = 0; // Minimum acceleration for particles
    public float MaxAcceleration { get; set; } = 300; // Maximum acceleration for particles
    public int Amount { get; set; } = 1; // Number of particles to generate per update

    private readonly Random random = new();

    private float NextFloat(float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }

    public override void Update()
    {
        base.Update();

        // Generate 'Amount' number of particles per update
        for (int i = 0; i < Amount; i++)
        {
            // Update the spawn area to match window size
            SpawnAreaMax = new(SpawnAreaMax.X, WindowManager.Size.Y);

            // Generate random position within the spawn area
            Vector2 randomPosition = new(
                NextFloat(SpawnAreaMin.X, SpawnAreaMax.X),
                NextFloat(SpawnAreaMin.Y, SpawnAreaMax.Y)
            );

            // Generate random acceleration within the specified range
            float randomAcceleration = NextFloat(MinAcceleration, MaxAcceleration);

            // Create a new particle with acceleration
            AddChild(new Particle()
            {
                Lifetime = Lifetime,
                Size = ParticleSize,
                Position = randomPosition,
                Acceleration = randomAcceleration
            });
        }
    }

    protected override void Draw()
    {
        base.Draw();

        DrawRectangle(Position, Size, Color.Red);
    }
}