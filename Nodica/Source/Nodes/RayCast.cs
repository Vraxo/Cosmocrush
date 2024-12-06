namespace Nodica;

public class RayCast : Node2D
{
    public float Length { get; set; } = 100f;

    public bool IsColliding { get; private set; } = false;
    public bool IgnoreFirst { get; set; } = false;

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

    public Vector2 TargetPosition => new(
        MathF.Cos(MathF.PI * Rotation / 180) * Length,
        MathF.Sin(MathF.PI * Rotation / 180) * Length
    );

    public override void Update()
    {
        PerformRaycast();
        base.Update();
    }

    protected override void Draw()
    {
        Vector2 rayEnd = GlobalPosition + TargetPosition;
        DrawLine(GlobalPosition, rayEnd, Color.Red);
        base.Draw();
    }

    private void PerformRaycast()
    {
        IsColliding = false;
        Collider = null;

        Vector2 rayStart = GlobalPosition;
        Vector2 rayEnd = rayStart + TargetPosition;

        float closestDistance = float.MaxValue;
        bool firstHitSkipped = false;

        foreach (Collider collider in CollisionManager.Instance.Colliders)
        {
            if (collider.RayIntersects(rayStart, rayEnd))
            {
                if (IgnoreFirst && !firstHitSkipped)
                {
                    firstHitSkipped = true;
                    continue;
                }

                float distance = Vector2.Distance(rayStart, collider.GlobalPosition);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    IsColliding = true;
                    Collider = collider;

                    Log.Info($"[RayCast] Hit {collider.Name}.", "RayCast");
                }
            }
        }
    }
}
