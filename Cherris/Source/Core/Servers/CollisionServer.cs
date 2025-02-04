namespace Cherris;

public sealed class CollisionServer
{
    private static CollisionServer? _instance;
    public static CollisionServer Instance => _instance ??= new();

    public List<Collider> Colliders = new();
    public List<ColliderCircle> ColliderCircles = new();
    public List<ColliderRectangle> ColliderRectangles = new();

    private CollisionServer() { }

    // Public

    public void RegisterCircle(ColliderCircle collider)
    {
        ColliderCircles.Add(collider);
        Colliders.Add(collider);
    }

    public void UnregisterCircle(ColliderCircle collider)
    {
        ColliderCircles.Remove(collider);
        Colliders.Remove(collider);
    }

    public void RegisterRectangle(ColliderRectangle collider)
    {
        ColliderRectangles.Add(collider);
        Colliders.Add(collider);
    }

    public void UnregisterRectangle(ColliderRectangle collider)
    {
        ColliderRectangles.Remove(collider);
        Colliders.Remove(collider);
    }

    public void Process()
    {
        ProcessCircleCollisions();
        ProcessRectangleCollisions();
    }

    public void PrintColliders()
    {
        Console.WriteLine("[CollisionServer - Colliders]");

        foreach (Collider collider in Colliders)
        {
            Console.WriteLine(collider.Name);
        }
    }

    // General

    private static bool AreLayersMatching(List<int> layersA, List<int> layersB)
    {
        foreach (var layerA in layersA)
        {
            if (layersB.Contains(layerA))
            {
                return true;
            }
        }
        return false;
    }

    // Circle collision logic

    private void ProcessCircleCollisions()
    {
        for (int i = 0; i < ColliderCircles.Count; i++)
        {
            for (int j = i + 1; j < ColliderCircles.Count; j++)
            {
                var colliderA = ColliderCircles[i];
                var colliderB = ColliderCircles[j];

                if (colliderA.Enabled && colliderB.Enabled && AreLayersMatching(colliderA.CollisionLayers, colliderB.CollisionLayers))
                {
                    HandleCircleCollision(colliderA, colliderB);
                }
            }
        }
    }

    private static void HandleCircleCollision(ColliderCircle colliderA, ColliderCircle colliderB)
    {
        float scaledRadiusA = colliderA.Radius * colliderA.Scale.X;
        float scaledRadiusB = colliderB.Radius * colliderB.Scale.X;
        float distance = (colliderA.GlobalPosition - colliderA.Origin).DistanceTo(colliderB.GlobalPosition - colliderB.Origin);
        float combinedRadii = scaledRadiusA + scaledRadiusB;

        if (distance < combinedRadii)
        {
            float overlap = combinedRadii - distance;
            ResolveCircleOverlap(colliderA, colliderB, overlap);
        }
    }

    private static void ResolveCircleOverlap(ColliderCircle colliderA, ColliderCircle colliderB, float overlap)
    {
        if (colliderA.IsStatic && colliderB.IsStatic) return;

        Vector2 direction = (colliderB.GlobalPosition - colliderB.Origin - colliderA.GlobalPosition + colliderA.Origin).Normalized();
        Vector2 pushVector = direction * overlap / 2;

        if (!colliderA.IsStatic) colliderA.Position -= pushVector;
        if (!colliderB.IsStatic) colliderB.Position += pushVector;
    }

    // Rectangle collision logic

    private void ProcessRectangleCollisions()
    {
        for (int i = 0; i < ColliderRectangles.Count; i++)
        {
            for (int j = i + 1; j < ColliderRectangles.Count; j++)
            {
                var colliderA = ColliderRectangles[i];
                var colliderB = ColliderRectangles[j];

                if (colliderA.Enabled && colliderB.Enabled && AreLayersMatching(colliderA.CollisionLayers, colliderB.CollisionLayers))
                {
                    HandleRectangleCollision(colliderA, colliderB);
                }
            }
        }
    }

    private static void HandleRectangleCollision(ColliderRectangle colliderA, ColliderRectangle colliderB)
    {
        if (!colliderA.Enabled || !colliderB.Enabled) return;

        if (IsRectangleColliding(colliderA, colliderB))
        {
            Vector2 overlap = GetRectangleOverlap(colliderA, colliderB);
            ResolveRectangleOverlap(colliderA, colliderB, overlap);
        }
    }

    private static bool IsRectangleColliding(ColliderRectangle colliderA, ColliderRectangle colliderB)
    {
        float scaledWidthA = colliderA.Size.X * colliderA.Scale.X;
        float scaledHeightA = colliderA.Size.Y * colliderA.Scale.Y;
        float scaledWidthB = colliderB.Size.X * colliderB.Scale.X;
        float scaledHeightB = colliderB.Size.Y * colliderB.Scale.Y;

        return colliderA.GlobalPosition.X < colliderB.GlobalPosition.X + scaledWidthB &&
               colliderA.GlobalPosition.X + scaledWidthA > colliderB.GlobalPosition.X &&
               colliderA.GlobalPosition.Y < colliderB.GlobalPosition.Y + scaledHeightB &&
               colliderA.GlobalPosition.Y + scaledHeightA > colliderB.GlobalPosition.Y;
    }

    private static Vector2 GetRectangleOverlap(ColliderRectangle colliderA, ColliderRectangle colliderB)
    {
        float overlapX = MathF.Min(colliderA.GlobalPosition.X + colliderA.Size.X, colliderB.GlobalPosition.X + colliderB.Size.X) -
                         MathF.Max(colliderA.GlobalPosition.X, colliderB.GlobalPosition.X);

        float overlapY = MathF.Min(colliderA.GlobalPosition.Y + colliderA.Size.Y, colliderB.GlobalPosition.Y + colliderB.Size.Y) -
                         MathF.Max(colliderA.GlobalPosition.Y, colliderB.GlobalPosition.Y);

        return new Vector2(overlapX, overlapY);
    }

    private static void ResolveRectangleOverlap(ColliderRectangle colliderA, ColliderRectangle colliderB, Vector2 overlap)
    {
        if (colliderA.IsStatic && colliderB.IsStatic) return;

        Vector2 pushVector = overlap / 2;
        if (!colliderA.IsStatic) colliderA.Position -= pushVector;
        if (!colliderB.IsStatic) colliderB.Position += pushVector;
    }
}
