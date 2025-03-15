using Cherris;

namespace Cosmocrush;

public class Player : RigidBody
{
    public int Health { get; private set; } = 100;

    private Vector2 knockbackVelocity = Vector2.Zero;

    private readonly Sprite? sprite;
    private readonly Sound? damageSound = ResourceLoader.Load<Sound>("Res/AudioStream/SFX/PlayerDamage.mp3");

    private const float Speed = 200f;
    private const float KnockbackRecoverySpeed = 0.1f;

    private Camera camera;

    public override void Ready()
    {
        base.Ready();
        //sprite!.Shader = Shader.Load(null, "Res/Shaders/Glow.shader");
        camera = GetNode<MainCamera>("/root/MainCamera");
    }

    public override void Process()
    {
        base.Process();

        SufferKnockback();
        LookAtMouse();
        HandleMovement();

        camera.Position = Position;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        damageSound?.Play("SFX");

        if (Health <= 0)
        {
            Die();
        }
    }

    public void ApplyKnockback(Vector2 knockback)
    {
        knockbackVelocity = knockbackVelocity.Length() < knockback.Length()
            ? knockback
            : knockbackVelocity + knockback;
    }

    private void LookAtMouse()
    {
        sprite!.FlipH = Input.WorldMousePosition.X <= GlobalPosition.X;
    }

    private void HandleMovement()
    {
        Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        LinearVelocity = (direction - knockbackVelocity) * Speed;
    }

    private void SufferKnockback()
    {
        knockbackVelocity = Vector2.Lerp(
            knockbackVelocity,
            Vector2.Zero,
            KnockbackRecoverySpeed);
    }

    private void Die()
    {
        Free();
    }
}