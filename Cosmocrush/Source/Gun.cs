using Cherris;

namespace Cosmocrush;

public class Gun : Sprite
{
    private RayCast rayCast = new();
    private readonly string gunshotAudioPath = "Res/Audio/SFX/Gunshot.mp3";

    private float lastFiredTime = 0f;
    private const float cooldown = 0.182f;
    private const float knockbackForce = 3f;

    public override void Ready()
    {
        base.Ready();

        HorizontalAlignment = HorizontalAlignment.Left;
        Offset = new(8, 0);
        Offset = new(0, 0);

        rayCast = GetNode<RayCast>("RayCast");
        rayCast.Deactivate();
    }

    public override void Update()
    {
        base.Update();

        HandleFiring();
        LookAtMouse();
    }

    private void HandleFiring()
    {
        bool isCooledDown = TimeManager.Elapsed - lastFiredTime >= cooldown;

        if (Input.IsActionDown("Fire") && isCooledDown)
        {
            Fire();
        }
    }

    private void LookAtMouse()
    {
        FlipV = Input.WorldMousePosition.X < GlobalPosition.X;
        LookAt(Input.WorldMousePosition);
    }

    private void PlayGunshotSound()
    {
        AudioPlayer newAudioPlayer = new()
        {
            Audio = ResourceLoader.Load<Audio>(gunshotAudioPath)
        };

        AddChild(newAudioPlayer);
        newAudioPlayer.Finished += OnGunshotAudioFinished;
        newAudioPlayer.Play();
    }

    private void OnGunshotAudioFinished(AudioPlayer sender)
    {
        sender.Destroy();
    }

    private void Fire()
    {
        lastFiredTime = TimeManager.Elapsed;
        PlayGunshotSound();
        FireRaycast();
    }

    private void FireRaycast()
    {
        Vector2 mousePosition = Input.WorldMousePosition;
        Vector2 angleVector = mousePosition - GlobalPosition;
        float angle = MathF.Atan2(angleVector.Y, angleVector.X);

        rayCast.Rotation = angle * 180 / MathF.PI;

        rayCast.GlobalPosition = GlobalPosition;

        rayCast.Update();

        if (rayCast.IsColliding)
        {
            Node2D? collider = rayCast.Collider;

            if (collider is not null)
            {
                if (collider is Enemy enemy)
                {
                    enemy.TakeDamage(1);
                    enemy.ApplyKnockback(angleVector.Normalized() * knockbackForce);
                }
            }
        }

        rayCast.Deactivate();
    }
}