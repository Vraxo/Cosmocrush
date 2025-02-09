using Cherris;

namespace Cosmocrush;

public class Gun : Sprite
{
    private readonly Sound gunshotSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/Gunshot.mp3");
    private readonly Sound reloadSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/Reload.mp3");
    private readonly PackedScene reloadProgressBarScene = new("Res/Scenes/ReloadProgressBar.yaml");

    private readonly RayCast? rayCast;
    private readonly Timer? cooldownTimer;
    private readonly Timer? reloadTimer;
    private ProgressBar? reloadProgressBar;

    private bool canFire = true;
    private int bulletsInMagazine = magazineSize;
    private bool reloading = false;
    private const int magazineSize = 10;
    private const int damage = 5;
    private const float knockbackForce = 3f;

    // Bloom-related variables
    private float currentBloom = 0f;
    private const float maxBloom = 0.1f;
    private const float bloomIncrease = 0.02f;
    private const float bloomResetSpeed = 0.05f;

    // Main

    public override void Ready()
    {
        base.Ready();

        rayCast!.Deactivate();

        cooldownTimer!.Timeout += OnCooldownTimerTimeout;
        reloadTimer!.Timeout += OnReloadTimerTimeout;
    }

    public override void Update()
    {
        base.Update();

        HandleFiring();
        HandleReloadingInput();
        LookAtMouse();

        if (reloading)
        {
            UpdateReloadProgressBar();
        }

        // Reset bloom when not firing
        if (!Input.IsActionDown("Fire"))
        {
            currentBloom = float.Max(0, currentBloom - bloomResetSpeed * TimeServer.Delta);
        }
    }

    // Timer event handlers

    private void OnCooldownTimerTimeout(Timer timer)
    {
        canFire = true;
    }

    private void OnReloadTimerTimeout(Timer timer)
    {
        bulletsInMagazine = magazineSize;
        reloading = false;
        Console.WriteLine("Reload complete. Magazine full.");
        RemoveReloadProgressBar();
    }

    // Input handling

    private void HandleFiring()
    {
        bool canShoot = canFire && !reloading && bulletsInMagazine > 0;

        if (Input.IsActionDown("Fire") && canShoot)
        {
            Fire();
        }
    }

    private void HandleReloadingInput()
    {
        if (Input.IsActionPressed("Reload") && !reloading && bulletsInMagazine < magazineSize)
        {
            StartReloading();
        }
    }

    private void Fire()
    {
        canFire = false;
        cooldownTimer!.Fire();

        bulletsInMagazine--;
        gunshotSound.Play("SFX");
        FireRaycast();

        // Increase bloom
        currentBloom = float.Min(maxBloom, currentBloom + bloomIncrease);

        if (bulletsInMagazine <= 0)
        {
            StartReloading();
        }
    }

    private void StartReloading()
    {
        reloading = true;
        reloadSound.Play("SFX");
        Console.WriteLine("Reloading initiated...");
        reloadTimer!.Fire();

        CreateReloadProgressBar();
    }

    private void LookAtMouse()
    {
        FlipV = Input.WorldMousePosition.X < GlobalPosition.X;
        LookAt(Input.WorldMousePosition);
    }

    private void FireRaycast()
    {
        Vector2 mousePosition = Input.WorldMousePosition;
        Vector2 angleVector = mousePosition - GlobalPosition;
        float angle = MathF.Atan2(angleVector.Y, angleVector.X);

        // Apply bloom to the angle
        angle += RandomRange(-currentBloom, currentBloom);

        rayCast!.Rotation = angle * 180 / MathF.PI;
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

    // Reload progress bar

    private void CreateReloadProgressBar()
    {
        if (reloadProgressBarScene is null)
        {
            return;
        }

        reloadProgressBar = reloadProgressBarScene.Instantiate<ProgressBar>();
        Parent!.AddChild(reloadProgressBar);
    }

    private void UpdateReloadProgressBar()
    {
        if (reloadProgressBar is null)
        {
            return;
        }

        float reloadProgress = 1.0f - (reloadTimer!.TimeLeft / reloadTimer!.WaitTime);
        reloadProgressBar.Percentage = reloadProgress;
    }

    private void RemoveReloadProgressBar()
    {
        if (reloadProgressBar is null)
        {
            return;
        }

        reloadProgressBar.Free();
        reloadProgressBar = null;
    }

    private static float RandomRange(float min, float max)
    {
        return (float)(new Random().NextDouble() * (max - min) + min);
    }
}