using Nodica;

namespace HordeRush;

public class Enemy : ColliderRectangle
{
    private readonly float speed = 100f;
    private readonly float proximityThreshold = 10f;
    private int health = 10;

    private NavigationAgent navigationAgent = new();
    private Sprite sprite = new();
    private Player player = new();

    private Vector2 knockback = Vector2.Zero;
    private readonly float knockbackRecoverySpeed = 0.1f;

    public override void Ready()
    {
        base.Ready();

        navigationAgent = GetNode<NavigationAgent>("NavigationAgent");
        navigationAgent.Region = GetNode<NavigationRegion>("/root/NavReg");
        sprite = GetNode<Sprite>("Sprite");
        player = GetNode<Player>("/root/Player");
    }

    public override void Update()
    {
        base.Update();

        knockback = Vector2.Lerp(knockback, Vector2.Zero, knockbackRecoverySpeed);
        ChasePlayer();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy();
    }

    private void ChasePlayer()
    {
        navigationAgent.TargetPosition = player.GlobalPosition;

        if (player == null || navigationAgent.Path.Count == 0)
        {
            return;
        }

        Vector2 targetPosition = navigationAgent.Path[0];
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();

        // Apply the knockback to the movement
        Vector2 movement = direction * speed * Time.Delta + knockback;
        GlobalPosition += movement;

        sprite.FlipH = !(direction.X < 0);

        if (GlobalPosition.DistanceTo(targetPosition) < proximityThreshold)
        {
            navigationAgent.Path.RemoveAt(0);
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        if (knockback.Length() < force.Length())
        {
            knockback = force;
        }
        else
        {
            knockback += force;
        }
    }
}