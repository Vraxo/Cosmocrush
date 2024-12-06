using Nodica;

namespace Cosmocrush;

public class Enemy : ColliderRectangle
{
    private Player player = new();
    private readonly float speed = 100f;
    private readonly float proximityThreshold = 10f;
    private int health = 1;

    public override void Start()
    {
        base.Start();
        Console.WriteLine(Name);

        Size = new(32, 32);
        player = GetNode<Player>("/root/MainScene/Player");
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
        if (player is null)
        {
            return;
        }

        Vector2 playerCenter = player.GlobalPosition + player.Size / 2;

        Vector2 direction = playerCenter - GlobalPosition;

        float distanceToPlayer = GlobalPosition.DistanceTo(playerCenter);

        if (distanceToPlayer > proximityThreshold)
        {
            direction = direction.Normalized();
            Vector2 newPosition = GlobalPosition + direction * speed * Time.Delta;
            Position = newPosition;
        }
        else
        {
            Position = GlobalPosition;
        }
    }
}