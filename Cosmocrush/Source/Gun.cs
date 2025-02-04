using Cherris;

namespace Cosmocrush;

public class Gun : Sprite
{
    private RayCast rayCast = new();
    private readonly string gunshotAudioPath = "Res/Audio/SFX/Gunshot.mp3";

    private float lastFiredTime = 0f;
    private const int damage = 5;
    private const float cooldown = 0.182f;
    private const float knockbackForce = 3f;

    // Main

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

    // Input

    private void HandleFiring()
    {
        bool isCooledDown = TimeServer.Elapsed - lastFiredTime >= cooldown;

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

    // Gunshot sound

    private void OnGunshotAudioFinished(AudioPlayer sender)
    {
        sender.Free();
    }

    private void PlayGunshotSound()
    {
        AudioPlayer newAudioPlayer = new()
        {
            Audio = ResourceLoader.Load<Audio>(gunshotAudioPath),
            Bus = "SFX"
        };

        AddChild(newAudioPlayer);
        newAudioPlayer.Finished += OnGunshotAudioFinished;
        newAudioPlayer.Play();
    }

    // Firing

    private void Fire()
    {
        lastFiredTime = TimeServer.Elapsed;
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
            Collider? collider = rayCast.Collider;

            if (collider is not null)
            {
                if (collider.Parent is Enemy enemy)
                {
                    enemy.TakeDamage(damage);
                    enemy.ApplyKnockback(angleVector.Normalized() * knockbackForce);
                }
            }
        }

        rayCast.Deactivate();
    }
}