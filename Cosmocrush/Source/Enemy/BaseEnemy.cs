using Cherris;

namespace Cosmocrush;

public abstract class BaseEnemy : RigidBody
{
    protected bool alive = true;
    protected int health;
    protected bool canShoot = true;

    protected Player? player;
    protected Sprite? sprite;
    protected NavigationAgent? navigationAgent;
    protected AnimationPlayer? hitFlashAnimationPlayer;
    protected ParticleEmitter? damageParticles;
    protected Timer? damageTimer;
    protected Sound? damageSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/EnemyDamage.mp3");

    protected Vector2 knockbackVelocity = Vector2.Zero;

    protected virtual int MaxHealth => 10;
    protected virtual int Damage => 2;
    protected virtual float Speed => 100f;
    protected virtual float ProximityThreshold => 32f;
    protected virtual float DamageRadius => 48f;
    protected virtual float DamageCooldown => 0.5f;
    protected virtual float KnockbackForce => 25f;
    protected virtual float EnemyKnockbackForce => 60f; // Increased knockback force
    protected virtual float KnockbackRecoverySpeed => 0.05f; // Slower recovery speed

    public override void Ready()
    {
        base.Ready();

        navigationAgent!.Region = GetNode<NavigationRegion>("/root/NavigationRegion");
        player = GetNode<Player>("/root/Player");
        health = MaxHealth;

        // Reduced LinearDamping to allow knockback to persist
        LinearDamping = 0.5f; // <--- CRITICAL FIX

        damageTimer!.Timeout += (timer) => canShoot = true;
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

    public void TakeDamage(int damage)
    {
        health -= damage;

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
        // Always add knockback to the existing velocity
        knockbackVelocity += knockback; // <--- CRITICAL FIX
    }

    protected virtual void ChasePlayer()
    {
        navigationAgent!.TargetPosition = player!.GlobalPosition;

        if (player is null || navigationAgent.Path.Count == 0)
        {
            return;
        }

        Vector2 movement = Vector2.Zero;

        // Only calculate movement if outside proximity threshold
        if (GlobalPosition.DistanceTo(player.GlobalPosition) >= ProximityThreshold)
        {
            Vector2 targetPosition = navigationAgent.Path[0];
            Vector2 direction = (targetPosition - GlobalPosition).Normalized();
            movement = direction * Speed;

            if (GlobalPosition.DistanceTo(targetPosition) < ProximityThreshold)
            {
                navigationAgent.Path.RemoveAt(0);
            }
        }

        // Always apply knockbackVelocity regardless of proximity
        LinearVelocity = movement + knockbackVelocity; // <--- CRITICAL FIX
    }

    protected void SufferKnockback()
    {
        // Gradually reduce knockback velocity over time
        knockbackVelocity = Vector2.Lerp(
            knockbackVelocity,
            Vector2.Zero,
            KnockbackRecoverySpeed);
    }

    protected void LookAtPlayer()
    {
        sprite!.FlipH = player!.GlobalPosition.X < GlobalPosition.X;
    }

    protected abstract void AttemptToDamagePlayer();

    protected void Die()
    {
        Enabled = false;

        hitFlashAnimationPlayer!.Stop();
        alive = false;

        foreach (Node child in Children.Where(child => child.Name != "DamageIndicator").ToList())
        {
            child.Free();
        }

        Tree.CreateTimer(1).Timeout += () => Free();
    }

    protected void CreateDamageIndicator(int damage)
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