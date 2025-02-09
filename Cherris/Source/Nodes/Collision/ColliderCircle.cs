namespace Cherris;

public class ColliderCircle : Collider
{
    public float Radius { get; set; } = 16;

    //public override bool RayIntersects(Vector2 rayStart, Vector2 rayEnd)
    //{
    //    Vector2 closestPoint = ClosestPointOnLine(rayStart, rayEnd, GlobalPosition);
    //    float distanceToCircle = Vector2.Distance(closestPoint, GlobalPosition);
    //
    //    return distanceToCircle <= Radius;
    //}

    public override void Draw()
    {
        base.Draw();

        DrawCircleOutline(
            GlobalPosition - Origin,
            Radius,
            Color.Blue);
    }

    protected override void Register()
    {
        CollisionServer.Instance.RegisterCircle(this);
    }

    protected override void Unregister()
    {
        CollisionServer.Instance.UnregisterCircle(this);
    }

    private static Vector2 ClosestPointOnLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        Vector2 line = lineEnd - lineStart;
        float t = Vector2.Dot(point - lineStart, line) / Vector2.Dot(line, line);
        t = Math.Clamp(t, 0, 1);
        return lineStart + t * line;
    }

    public override float? GetIntersection(Vector2 rayStart, Vector2 rayEnd)
    {
        throw new NotImplementedException();
    }
}