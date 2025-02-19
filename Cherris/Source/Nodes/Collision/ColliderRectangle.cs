namespace Cherris;

public class ColliderRectangle : Collider
{
    public override void Draw()
    {
        DrawRectangleOutline(
            GlobalPosition - Origin,
            ScaledSize,
            Color
        );
    }

    public override float? GetIntersection(Vector2 rayStart, Vector2 rayEnd)
    {
        Vector2 rectMin = GlobalPosition - Origin;
        Vector2 rectMax = rectMin + ScaledSize;

        float? closestT = null;
        CheckEdge(rayStart, rayEnd, rectMin, new Vector2(rectMax.X, rectMin.Y), ref closestT);
        CheckEdge(rayStart, rayEnd, new Vector2(rectMax.X, rectMin.Y), rectMax, ref closestT);
        CheckEdge(rayStart, rayEnd, rectMax, new Vector2(rectMin.X, rectMax.Y), ref closestT);
        CheckEdge(rayStart, rayEnd, new Vector2(rectMin.X, rectMax.Y), rectMin, ref closestT);

        return closestT;
    }

    private static void CheckEdge(Vector2 rayStart, Vector2 rayEnd, Vector2 edgeA, Vector2 edgeB, ref float? closestT)
    {
        Vector2 rayDir = rayEnd - rayStart;
        Vector2 edgeDir = edgeB - edgeA;

        float denominator = rayDir.X * edgeDir.Y - rayDir.Y * edgeDir.X;
        if (denominator == 0) return;

        Vector2 delta = edgeA - rayStart;
        float t = (delta.X * edgeDir.Y - delta.Y * edgeDir.X) / denominator;
        float u = (delta.X * rayDir.Y - delta.Y * rayDir.X) / denominator;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1 && (t < closestT || !closestT.HasValue))
        {
            closestT = t;
        }
    }

    protected override void Register()
    {
        CollisionServer.Instance.RegisterRectangle(this);
    }

    protected override void Unregister()
    {
        CollisionServer.Instance.UnregisterRectangle(this);
    }
}