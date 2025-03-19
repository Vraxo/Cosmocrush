using Cherris;

namespace Cosmocrush;

public class Projectile : Area2D
{
    public float Speed { get; set; } = 500f;
    public float Direction { get; set; } = 0;

    public override void Ready()
    {
        base.Ready();

        BodyEntered += OnBodyEntered;

        Vector2 directionVector = new(
            float.Cos(float.DegreesToRadians(Direction)),
            float.Sin(float.DegreesToRadians(Direction))
        );

        LinearVelocity = directionVector * Speed;
    }

    private void OnBodyEntered(Node otherBody)
    {
        if (otherBody is not Player player)
        {
            return;
        }

        player.TakeDamage(1);

        //Vector2 knockbackDirection = (GlobalPosition - player!.GlobalPosition).Normalized();
        //player.ApplyKnockback(knockbackDirection * KnockbackForce);

        Destroy();
    }

    private void Destroy()
    {
        Free();
    }
}