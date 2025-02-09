namespace Cherris;

public class RayCast : Node2D
{
    public List<int> CollisionLayers { get; set; } = [0];
    public float Length { get; set; } = 100f;
    public bool IgnoreFirst { get; set; } = false;

    public bool IsColliding { get; private set; } = false;
    public Vector2 CollisionPoint { get; private set; } = Vector2.Zero;

    private Collider? collider;
    public Collider? Collider
    {
        get => collider;
        private set => collider = value;
    }

    public Vector2 TargetPosition => new(
        MathF.Cos(MathF.PI * Rotation / 180) * Length,
        MathF.Sin(MathF.PI * Rotation / 180) * Length
    );

    public RayCast()
    {
        ReadyForVisibility = true;
    }

    public override void Update()
    {
        PerformRaycast();
        base.Update();
    }

    public override void Draw()
    {
        DrawLine(
            GlobalPosition,
            GlobalPosition + TargetPosition,
            5,
            Color.Red);
    }

    private void PerformRaycast()
    {
        ResetCollisionData();

        Vector2 rayStart = GlobalPosition;
        Vector2 rayEnd = rayStart + TargetPosition;
        Vector2 rayDirection = (rayEnd - rayStart).Normalized();

        var hits = CollectPotentialHits(rayStart, rayEnd, rayDirection);
        ProcessCollisionResults(hits);
    }

    private void ResetCollisionData()
    {
        IsColliding = false;
        Collider = null;
        CollisionPoint = Vector2.Zero;
    }

    private List<(Collider collider, float distance, Vector2 point)> CollectPotentialHits(Vector2 rayStart, Vector2 rayEnd, Vector2 rayDirection)
    {
        var hits = new List<(Collider, float, Vector2)>();

        foreach (var collider in CollisionServer.Instance.Colliders)
        {
            if (!IsValidCollider(collider)) continue;

            if (collider.RayIntersects(rayStart, rayEnd))
            {
                var hit = CalculateHitDetails(collider, rayStart, rayDirection);
                hits.Add(hit);
            }
        }

        return hits;
    }

    private bool IsValidCollider(Collider collider)
    {
        return collider.Enabled &&
               CollisionLayers.Any(layer => collider.CollisionLayers.Contains(layer));
    }

    private (Collider collider, float distance, Vector2 point) CalculateHitDetails(Collider collider, Vector2 rayStart, Vector2 rayDirection)
    {
        Vector2 toCollider = collider.GlobalPosition - rayStart;
        float distance = Math.Clamp(Vector2.Dot(toCollider, rayDirection), 0f, Length);
        Vector2 point = rayStart + rayDirection * distance;
        return (collider, distance, point);
    }

    private void ProcessCollisionResults(List<(Collider collider, float distance, Vector2 point)> hits)
    {
        hits.Sort((a, b) => a.distance.CompareTo(b.distance));
        bool skipFirst = IgnoreFirst;

        foreach (var hit in hits)
        {
            if (skipFirst)
            {
                Console.WriteLine("skipped first: " + hit.collider.Name);
                skipFirst = false;
                continue;
            }

            SetCollisionData(hit);
            return;
        }
    }

    private void SetCollisionData((Collider collider, float distance, Vector2 point) hit)
    {
        IsColliding = true;
        Collider = hit.collider;
        CollisionPoint = hit.point;
        Console.WriteLine($"[RayCast] [{Name}] Hit {Collider.AbsolutePath} at {CollisionPoint}.", "RayCast");
    }
}