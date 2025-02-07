using Cherris;

namespace Cosmocrush;

public class Player : ColliderRectangle
{
    public int Health { get; private set; } = 100;

    private readonly float speed = 200f;
    private readonly Sprite? sprite;
    private readonly Sound? damageSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/PlayerDamage.mp3");

    public override void Update()
    {
        base.Update();

        LookAtMouse();
        HandleMovement();
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

    private void LookAtMouse()
    {
        sprite!.FlipH = Input.WorldMousePosition.X <= GlobalPosition.X;
    }

    private void HandleMovement()
    {
        Vector2 direction = Input.GetVector(
            "MoveLeft",
            "MoveRight",
            "MoveUp",
            "MoveDown");

        Position += direction * TimeServer.Delta * speed;
    }

    private void Die()
    {
        Free();
    }
}