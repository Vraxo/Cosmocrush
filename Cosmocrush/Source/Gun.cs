using Cherris;

namespace Cosmocrush;

public class Gun : Sprite
{
    private readonly Sound gunshotSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/Gunshot.mp3");
    private readonly Sound reloadSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/Reload.mp3");

    private const int damage = 5;
    private const float knockbackForce = 3f;

    private readonly RayCast? rayCast;
    private readonly Timer? cooldownTimer;
    private readonly Timer? reloadTimer;

    private bool canFire = true;
    private int bulletsInMagazine;
    private bool reloading = false;
    private const int magazineSize = 10;

    // Main

    public override void Ready()
    {
        base.Ready();

        HorizontalAlignment = HorizontalAlignment.Left;
        Offset = new(8, 0);
        Offset = new(0, 0);

        rayCast!.Deactivate();

        bulletsInMagazine = magazineSize;

        cooldownTimer!.Timeout += OnCooldownTimerTimeout;
        reloadTimer!.Timeout += OnReloadTimerTimeout;
    }

    public override void Update()
    {
        base.Update();

        HandleFiring();
        HandleReloadingInput();
        LookAtMouse();
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

        // Auto-reload if the magazine is empty.
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
}