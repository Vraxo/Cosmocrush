using Cherris;

namespace Cosmocrush;

public class Player : ColliderRectangle
{
    public int Health { get; private set; } = 100;

    private readonly Sprite? sprite;
    private readonly Sound? damageSound = ResourceLoader.Load<Sound>("Res/AudioStream/SFX/PlayerDamage.mp3");

    private const float Speed = 200f;

    // Knockback variables
    private Vector2 knockbackVelocity = Vector2.Zero;
    private const float knockbackRecoverySpeed = 0.1f;

    public override void Ready()
    {
        base.Ready();

        sprite!.Shader = Shader.Load(null, "Res/Shaders/Glow.shader");
    }

    public override void Process()
    {
        base.Process();

        SufferKnockback();
        LookAtMouse();
        HandleMovement();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        damageSound?.Play("SFX");

        if (Health <= 0) Die();
    }

    public void ApplyKnockback(Vector2 knockback)
    {
        if (knockbackVelocity.Length() < knockback.Length())
        {
            knockbackVelocity = knockback;
        }
        else
        {
            knockbackVelocity += knockback;
        }
    }

    private void LookAtMouse()
    {
        sprite!.FlipH = Input.WorldMousePosition.X <= GlobalPosition.X;
    }

    private void HandleMovement()
    {
        Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        Vector2 movement = direction * Speed + knockbackVelocity;
        Position += movement * TimeServer.Delta; // Delta applied once to combined forces
    }

    private void SufferKnockback()
    {
        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.Zero, knockbackRecoverySpeed);
    }

    private void Die() => Free();
}