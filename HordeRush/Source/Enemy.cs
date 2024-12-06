using Nodica;

namespace HordeRush;

public class Enemy : ColliderRectangle
{
    private readonly float speed = 100f;  // You can adjust the speed as needed
    private readonly float proximityThreshold = 10f;  // Threshold to switch to next path point
    private int health = 1;

    private NavigationAgent navigationAgent = new();
    private Player player = new();

    public override void Ready()
    {
        base.Ready();

        Size = new(32, 32);
        navigationAgent = GetNode<NavigationAgent>("NavigationAgent");
        player = GetNode<Player>("/root/Player");
    }

    public override void Update()
    {
        base.Update();

        // Update the navigation path to the player position
        navigationAgent.TargetPosition = player.GlobalPosition;
        navigationAgent.Update();

        // Move towards the next path point
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

        // Get the current target point (next point in the path)
        Vector2 targetPosition = navigationAgent.Path[0];

        // Calculate the direction to the target point
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();

        // Move towards the target point
        GlobalPosition += direction * speed * Time.Delta;

        // Check if the enemy is close enough to the target point
        if (GlobalPosition.DistanceTo(targetPosition) < proximityThreshold)
        {
            // Remove the target point from the path, move to the next one
            navigationAgent.Path.RemoveAt(0);
        }
    }
}
