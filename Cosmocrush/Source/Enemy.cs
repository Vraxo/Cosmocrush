using Cherris;

namespace Cosmocrush;

public class Enemy : ColliderRectangle
{
    private int health = 20;
    private const int maxHealth = 20;
    private Vector2 knockback = Vector2.Zero;
    private double lastDamageTime = -0.5f;

    private Sprite? sprite;
    private Player? player;
    private NavigationAgent? navigationAgent;

    private readonly int damage = 2;
    private readonly float speed = 100f;
    private readonly float proximityThreshold = 10f;
    private readonly float knockbackRecoverySpeed = 0.1f;
    private readonly float damageRadius = 100;
    private readonly float damageCooldown = 0.5f;

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

        SufferKnockback();
        ChasePlayer();
        LookAtPlayer();
        AttemptToDamagePlayer();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }

        CreateDamageIndicator(damage);
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

    private void CreateDamageIndicator(int damage)
    {
        PackedScene damageIndicatorScene = new("Res/Scenes/DamageIndicator.yaml");
        var damageIndicator = damageIndicatorScene.Instantiate<DamageIndicator>();

        damageIndicator.Text = damage.ToString();
        damageIndicator.Health = health;
        damageIndicator.MaxHealth = maxHealth;

        AddChild(damageIndicator);
    }

    private void LookAtPlayer()
    {
        sprite!.FlipH = player!.GlobalPosition.X < GlobalPosition.X;
    }

    private void SufferKnockback()
    {
        knockback = Vector2.Lerp(knockback, Vector2.Zero, knockbackRecoverySpeed);
    }

    private void ChasePlayer()
    {
        navigationAgent!.TargetPosition = player!.GlobalPosition;

        if (player is null || navigationAgent.Path.Count == 0)
        {
            return;
        }

        Vector2 targetPosition = navigationAgent.Path[0];
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();

        Vector2 movement = direction * speed * TimeServer.Delta + knockback;
        GlobalPosition += movement;


        if (GlobalPosition.DistanceTo(targetPosition) < proximityThreshold)
        {
            navigationAgent.Path.RemoveAt(0);
        }
    }

    private void AttemptToDamagePlayer()
    {
        bool isPlayerInRange = GlobalPosition.DistanceTo(player.GlobalPosition) <= damageRadius;
        bool canShoot = TimeServer.Elapsed - lastDamageTime >= damageCooldown;

        if (isPlayerInRange && canShoot)
        {
            player?.TakeDamage(damage);
            lastDamageTime = TimeServer.Elapsed;
        }
    }

    private void Die()
    {
        Free();
    }
}