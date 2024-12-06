namespace Nodica;

public sealed class CollisionManager
{
    private static CollisionManager? _instance;
    public static CollisionManager Instance => _instance ??= new();

    public List<Collider> Colliders = [];
    public List<ColliderCircle> ColliderCircles = [];
    public List<ColliderRectangle> ColliderRectangles = [];

    private CollisionManager() { }

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

    private void ProcessCircleCollisions()
    {
        for (int i = 0; i < ColliderCircles.Count; i++)
        {
            for (int j = i + 1; j < ColliderCircles.Count; j++)
            {
                HandleCircleCollision(ColliderCircles[i], ColliderCircles[j]);
            }
        }
    }

    private void HandleCircleCollision(ColliderCircle colliderA, ColliderCircle colliderB)
    {
        float distance = colliderA.GlobalPosition.DistanceTo(colliderB.GlobalPosition);
        float combinedRadii = colliderA.Radius + colliderB.Radius;

        if (distance < combinedRadii)
        {
            float overlap = combinedRadii - distance;
            ResolveCircleOverlap(colliderA, colliderB, overlap);
        }
    }

    private void ResolveCircleOverlap(ColliderCircle colliderA, ColliderCircle colliderB, float overlap)
    {
        Vector2 direction = (colliderB.GlobalPosition - colliderA.GlobalPosition).Normalized();
        Vector2 pushVector = direction * overlap / 2;

        colliderA.Position -= pushVector;
        colliderB.Position += pushVector;
    }

    private void ProcessRectangleCollisions()
    {
        for (int i = 0; i < ColliderRectangles.Count; i++)
        {
            for (int j = i + 1; j < ColliderRectangles.Count; j++)
            {
                HandleRectangleCollision(ColliderRectangles[i], ColliderRectangles[j]);
            }
        }
    }

    private void HandleRectangleCollision(ColliderRectangle colliderA, ColliderRectangle colliderB)
    {
        if (IsRectangleColliding(colliderA, colliderB))
        {
            Vector2 overlap = GetRectangleOverlap(colliderA, colliderB);
            ResolveRectangleOverlap(colliderA, colliderB, overlap);
        }
    }

    private bool IsRectangleColliding(ColliderRectangle colliderA, ColliderRectangle colliderB)
    {
        return colliderA.GlobalPosition.X < colliderB.GlobalPosition.X + colliderB.Size.X &&
               colliderA.GlobalPosition.X + colliderA.Size.X > colliderB.GlobalPosition.X &&
               colliderA.GlobalPosition.Y < colliderB.GlobalPosition.Y + colliderB.Size.Y &&
               colliderA.GlobalPosition.Y + colliderA.Size.Y > colliderB.GlobalPosition.Y;
    }

    private Vector2 GetRectangleOverlap(ColliderRectangle colliderA, ColliderRectangle colliderB)
    {
        float overlapX = MathF.Min(colliderA.GlobalPosition.X + colliderA.Size.X, colliderB.GlobalPosition.X + colliderB.Size.X) -
                         MathF.Max(colliderA.GlobalPosition.X, colliderB.GlobalPosition.X);

        float overlapY = MathF.Min(colliderA.GlobalPosition.Y + colliderA.Size.Y, colliderB.GlobalPosition.Y + colliderB.Size.Y) -
                         MathF.Max(colliderA.GlobalPosition.Y, colliderB.GlobalPosition.Y);

        return new Vector2(overlapX, overlapY);
    }

    private void ResolveRectangleOverlap(ColliderRectangle colliderA, ColliderRectangle colliderB, Vector2 overlap)
    {
        if (overlap.X < overlap.Y)
        {
            ResolveHorizontalOverlap(colliderA, colliderB, overlap.X);
        }
        else
        {
            ResolveVerticalOverlap(colliderA, colliderB, overlap.Y);
        }
    }

    private void ResolveHorizontalOverlap(ColliderRectangle colliderA, ColliderRectangle colliderB, float overlapX)
    {
        float pushVectorX = overlapX / 2;
        if (colliderA.GlobalPosition.X < colliderB.GlobalPosition.X)
        {
            colliderA.Position -= new Vector2(pushVectorX, 0);
            colliderB.Position += new Vector2(pushVectorX, 0);
        }
        else
        {
            colliderA.Position += new Vector2(pushVectorX, 0);
            colliderB.Position -= new Vector2(pushVectorX, 0);
        }
    }

    private void ResolveVerticalOverlap(ColliderRectangle colliderA, ColliderRectangle colliderB, float overlapY)
    {
        float pushVectorY = overlapY / 2;
        if (colliderA.GlobalPosition.Y < colliderB.GlobalPosition.Y)
        {
            colliderA.Position -= new Vector2(0, pushVectorY);
            colliderB.Position += new Vector2(0, pushVectorY);
        }
        else
        {
            colliderA.Position += new Vector2(0, pushVectorY);
            colliderB.Position -= new Vector2(0, pushVectorY);
        }
    }
}