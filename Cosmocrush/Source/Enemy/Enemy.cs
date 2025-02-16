using Cherris;

namespace Cosmocrush;

public class Enemy : ColliderRectangle
{
    private int health = MaxHealth;
    private Vector2 knockback = Vector2.Zero;
    private double lastDamageTime = -0.5f;
    private bool alive = true;

    private Player? player;
    private readonly Sprite? sprite;
    private readonly NavigationAgent? navigationAgent;
    private readonly ColliderRectangle? hitBox;
    private readonly AnimationPlayer? hitFlashAnimationPlayer;
    private readonly ParticleEmitter? damageParticles;

    private readonly Sound damageSound = ResourceLoader.Load<Sound>("Res/AudioStream/SFX/EnemyDamage.mp3");

    private const int MaxHealth = 2000;
    private const int Damage = 2;
    //private const float Speed = 100f;
    private const float Speed = 0f;
    private const float ProximityThreshold = 10f;
    private const float KnockbackRecoverySpeed = 0.1f;
    private const float DamageRadius = 100;
    private const float DamageCooldown = 0.5f;

    // Main

    public override void Ready()
    {
        base.Ready();

        ProcessingMode = ProcessMode.Disabled;
        navigationAgent!.Region = GetNode<NavigationRegion>("/root/NavigationRegion");
        player = GetNode<Player>("/root/Player");
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

        CreateDamageIndicator(damage);
        hitFlashAnimationPlayer!.Play("Res/Animations/HitFlash.anim.yaml");
        damageParticles!.Emitting = true;

        if (health <= 0)
        {
            Die();
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

    // Take Damage

    private void SufferKnockback()
    {
        knockback = Vector2.Lerp(knockback, Vector2.Zero, KnockbackRecoverySpeed);
    }

    private void CreateDamageIndicator(int damage)
    {
        damageSound.Play("SFX");

        PackedScene damageIndicatorScene = new("Res/Scenes/DamageIndicator.yaml");
        var damageIndicator = damageIndicatorScene.Instantiate<DamageIndicator>();

        damageIndicator.Text = damage.ToString();
        damageIndicator.Health = health;
        damageIndicator.MaxHealth = MaxHealth;

        AddChild(damageIndicator);
    }

    private void Die()
    {
        hitFlashAnimationPlayer!.Stop();

        alive = false;
        Enabled = false;

        foreach (Node child in Children.Where(child => child.Name != "DamageIndicator").ToList())
        {
            child.Free();
        }

        Tree.CreateTimer(1).Timeout += () => Free();
    }

    // Follow player

    private void LookAtPlayer()
    {
        sprite!.FlipH = player!.GlobalPosition.X < GlobalPosition.X;
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

        Vector2 movement = direction * Speed * TimeServer.Delta + knockback;
        GlobalPosition += movement;


        if (GlobalPosition.DistanceTo(targetPosition) < ProximityThreshold)
        {
            navigationAgent.Path.RemoveAt(0);
        }
    }

    private void AttemptToDamagePlayer()
    {
        bool isPlayerInRange = GlobalPosition.DistanceTo(player!.GlobalPosition) <= DamageRadius;
        bool canShoot = TimeServer.Elapsed - lastDamageTime >= DamageCooldown;

        if (isPlayerInRange && canShoot)
        {
            player?.TakeDamage(Damage);
            lastDamageTime = TimeServer.Elapsed;
        }
    }
}