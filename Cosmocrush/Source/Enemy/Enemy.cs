using Cherris;

namespace Cosmocrush;

public class Enemy : RigidBody
{
    private int health = MaxHealth;
    private double lastDamageTime = -0.5f;
    private bool alive = true;

    private Player? player;
    private readonly Sprite? sprite;
    private readonly NavigationAgent? navigationAgent;
    private readonly AnimationPlayer? hitFlashAnimationPlayer;
    private readonly ParticleEmitter? damageParticles;

    private readonly Sound? damageSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/EnemyDamage.mp3");

    private Vector2 knockbackVelocity = Vector2.Zero;

    private const int Damage = 2;
    private const int MaxHealth = 2000;
    private const float Speed = 100f;
    private const float ProximityThreshold = 32f;
    private const float DamageRadius = 48;
    private const float DamageCooldown = 0.5f;
    private const float KnockbackForce = 25;
    private const float EnemyKnockbackForce = 40f;
    private const float KnockbackRecoverySpeed = 0.1f;

    // Main

    public override void Ready()
    {
        base.Ready();

        navigationAgent!.Region = GetNode<NavigationRegion>("/root/NavigationRegion");
        player = GetNode<Player>("/root/Player");
        LinearDamping = 10f;
    }

    public override void Process()
    {
        base.Process();

        if (!alive)
        {
            return;
        }

        SufferKnockback();
        ChasePlayer();
        LookAtPlayer();
        AttemptToDamagePlayer();
    }

    // Public

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Apply knockback from damage source (player)
        if (player != null)
        {
            Vector2 knockbackDirection = (GlobalPosition - player.GlobalPosition).Normalized();
            ApplyKnockback(knockbackDirection * EnemyKnockbackForce);
        }

        CreateDamageIndicator(damage);
        hitFlashAnimationPlayer!.Play("Res/Animations/HitFlash.anim.yaml");
        damageParticles!.Emitting = true;

        if (health <= 0)
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

    // Behavior

    private void ChasePlayer()
    {
        navigationAgent!.TargetPosition = player!.GlobalPosition;

        if (player is null || navigationAgent.Path.Count == 0)
        {
            return;
        }

        Vector2 targetPosition = navigationAgent.Path[0];
        Vector2 direction = (targetPosition - GlobalPosition).Normalized();
        Vector2 movement = direction * Speed;

        LinearVelocity = movement + knockbackVelocity;

        if (GlobalPosition.DistanceTo(targetPosition) < ProximityThreshold)
        {
            navigationAgent.Path.RemoveAt(0);
        }
    }

    private void SufferKnockback()
    {
        knockbackVelocity = Vector2.Lerp(
            knockbackVelocity,
            Vector2.Zero,
            KnockbackRecoverySpeed);
    }

    private void LookAtPlayer()
    {
        sprite!.FlipH = player!.GlobalPosition.X < GlobalPosition.X;
    }

    private void AttemptToDamagePlayer()
    {
        bool isPlayerInRange = GlobalPosition.DistanceTo(player!.GlobalPosition) <= DamageRadius;
        bool canShoot = TimeServer.Elapsed - lastDamageTime >= DamageCooldown;

        if (!isPlayerInRange || !canShoot)
        {
            return;
        }

        player?.TakeDamage(Damage);

        Vector2 knockbackDirection = (GlobalPosition - player!.GlobalPosition).Normalized();
        player.ApplyKnockback(knockbackDirection * KnockbackForce);

        lastDamageTime = TimeServer.Elapsed;
    }

    // Damage

    private void Die()
    {
        hitFlashAnimationPlayer!.Stop();
        alive = false;

        foreach (Node child in Children.Where(child => child.Name != "DamageIndicator").ToList())
        {
            child.Free();
        }

        Tree.CreateTimer(1).Timeout += () => Free();
    }

    private void CreateDamageIndicator(int damage)
    {
        damageSound?.Play("SFX");

        PackedScene damageIndicatorScene = new("Res/Scenes/DamageIndicator.yaml");
        var damageIndicator = damageIndicatorScene.Instantiate<DamageIndicator>();

        damageIndicator.Text = damage.ToString();
        damageIndicator.Health = health;
        damageIndicator.MaxHealth = MaxHealth;

        AddChild(damageIndicator);
    }
}