namespace Cherris;

public class RayCast : Node2D
{
    public List<int> CollisionLayers { get; set; } = [0];
    public float Length { get; set; } = 100f;
    public bool IgnoreFirst { get; set; } = false;

    public bool IsColliding { get; private set; } = false;

    private Collider? _collider;
    public Collider? Collider
    {
        get => _collider;
        private set
        {
            if (_collider != value)
            {
                _collider = value;
            }
        }
    }

    public Vector2 TargetPosition
    {
        get
        {
            float x = MathF.Cos(MathF.PI * Rotation / 180) * Length;
            float y = MathF.Sin(MathF.PI * Rotation / 180) * Length;
            return new(x, y);
        }
    }

    // Public

    public RayCast()
    {
        ReadyForVisibility = true;
    }

    public override void Update()
    {
        PerformRaycast();
        base.Update();
    }

    // Protected

    public override void Draw()
    {
        base.Draw();
        Vector2 rayEnd = GlobalPosition + TargetPosition;
        DrawLine(GlobalPosition, rayEnd, 5, Color.Red);
    }

    // Private

    private void PerformRaycast()
    {
        IsColliding = false;
        Collider = null;

        Vector2 rayStart = GlobalPosition;
        Vector2 rayEnd = rayStart + TargetPosition;

        bool firstHitSkipped = false;
        List<(Collider collider, float distance)> hits = [];

        foreach (Collider collider in CollisionServer.Instance.Colliders)
        {
            // Ensure collider is enabled and there's at least one common collision layer
            if (!collider.Enabled || !CollisionLayers.Any(layer => collider.CollisionLayers.Contains(layer)))
                continue;

            // Check if the ray intersects
            if (collider.RayIntersects(rayStart, rayEnd))
            {
                float distance = Vector2.Distance(rayStart, collider.GlobalPosition);
                hits.Add((collider, distance));
            }
        }

        // Sort hits by distance (closest first)
        hits.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Loop through sorted hits and apply IgnoreFirst logic
        foreach (var (collider, distance) in hits)
        {
            if (IgnoreFirst && !firstHitSkipped)
            {
                firstHitSkipped = true;
                continue; // Skip the first valid hit
            }

            // Once we find the closest valid hit, mark it
            IsColliding = true;
            Collider = collider;
            Log.Info($"[RayCast] [{Name}] Hit {Collider.Name}.", "RayCast");
            break; // Stop after finding the first valid hit
        }
    }
}