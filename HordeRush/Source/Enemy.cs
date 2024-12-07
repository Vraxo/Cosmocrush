using Nodica;

namespace HordeRush;

public class Enemy : ColliderRectangle
{
    private readonly float speed = 100f;
    private readonly float proximityThreshold = 10f;
    private int health = 1;

    private NavigationAgent navigationAgent = new();
    private Sprite sprite = new();
    private Player player = new();

    public override void Ready()
    {
        base.Ready();

        Size = new(32, 32);
        navigationAgent = GetNode<NavigationAgent>("NavigationAgent");
        navigationAgent.Region = GetNode<NavigationRegion>("/root/NavReg");
        sprite = GetNode<Sprite>("Sprite");
        player = GetNode<Player>("/root/Player");
    }

    public override void Update()
    {
        base.Update();

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
        if (player == null || navigationAgent.Path.Count == 0)
        {
            return;
        }

        navigationAgent.TargetPosition = player.GlobalPosition;

        Vector2 targetPosition = navigationAgent.Path[0];
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * speed * Time.Delta;

        sprite.FlipH = !(direction.X < 0);

        if (GlobalPosition.DistanceTo(targetPosition) < proximityThreshold)
        {
            navigationAgent.Path.RemoveAt(0);
        }
    }
}