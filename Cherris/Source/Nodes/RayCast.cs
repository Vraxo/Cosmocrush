namespace Cherris;

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class RayCast : Node2D
{
    public List<int> CollisionLayers { get; set; } = new() { 0 };
    public float Length { get; set; } = 100f;
    public bool IgnoreFirst { get; set; } = false;
    public bool IsColliding { get; private set; }
    public Vector2 CollisionPoint { get; private set; }
    public Collider? Collider { get; private set; }

    public Vector2 TargetPosition => new Vector2(
        MathF.Cos(Rotation * MathF.PI / 180f),
        MathF.Sin(Rotation * MathF.PI / 180f)
    ) * Length;

    public override void Process()
    {
        base.Process();
        Perform();
    }

    public override void Draw()
    {
        //DrawLine(GlobalPosition, GlobalPosition + TargetPosition, 2, Color.Red);
    }

    private void Perform()
    {
        IsColliding = false;
        Collider = null;
        CollisionPoint = Vector2.Zero;

        Vector2 rayStart = GlobalPosition;
        Vector2 rayEnd = rayStart + TargetPosition;
        var hits = new List<(Collider collider, float t)>();

        foreach (var collider in CollisionServer.Instance.Colliders)
        {
            if (!collider.Enabled || !CollisionLayers.Intersect(collider.CollisionLayers).Any())
                continue;

            float? t = collider.GetIntersection(rayStart, rayEnd);
            if (t.HasValue) hits.Add((collider, t.Value));
        }

        foreach (var hit in hits.OrderBy(h => h.t))
        {
            if (IgnoreFirst)
            {
                IgnoreFirst = false;
                continue;
            }

            IsColliding = true;
            Collider = hit.collider;
            Console.WriteLine("Hit " + Collider.AbsolutePath);
            CollisionPoint = Vector2.Lerp(rayStart, rayEnd, hit.t);
            return;
        }
    }
}