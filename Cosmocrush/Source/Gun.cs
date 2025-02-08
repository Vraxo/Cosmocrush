using Cherris;

namespace Cosmocrush;

public class Gun : Sprite
{
    private RayCast rayCast = new();
    private readonly Sound gunshotSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/Gunshot.mp3");
    private readonly Sound reloadSound = ResourceLoader.Load<Sound>("Res/Audio/SFX/Reload.mp3");

    private float lastFiredTime = 0f;
    private const int damage = 5;
    private const float cooldown = 0.182f;
    private const float knockbackForce = 3f;

    // New magazine system
    private const int magazineSize = 10;
    private int bulletsInMagazine;
    private bool isReloading = false;
    private float reloadStartTime = 0f;
    private const float reloadTime = 2.0f;

    public override void Ready()
    {
        base.Ready();

        HorizontalAlignment = HorizontalAlignment.Left;
        Offset = new(8, 0);
        Offset = new(0, 0);

        rayCast = GetNode<RayCast>("RayCast");
        rayCast.Deactivate();

        // Initialize magazine
        bulletsInMagazine = magazineSize;
    }

    public override void Update()
    {
        base.Update();

        HandleReloading();
        HandleFiring();
        LookAtMouse();
    }

    private void HandleFiring()
    {
        bool isCooledDown = TimeServer.Elapsed - lastFiredTime >= cooldown;
        bool canFire = isCooledDown && !isReloading && bulletsInMagazine > 0;

        if (Input.IsActionDown("Fire") && canFire)
        {
            Fire();
        }
    }

    private void Fire()
    {
        lastFiredTime = TimeServer.Elapsed;
        bulletsInMagazine--;
        gunshotSound.Play("SFX");
        FireRaycast();

        // Start reload automatically when empty
        if (bulletsInMagazine <= 0)
        {
            StartReloading();
        }
    }

    private void StartReloading()
    {
        isReloading = true;
        reloadStartTime = TimeServer.Elapsed;
        reloadSound.Play("SFX");
        Console.WriteLine("Reloading initiated...");
    }

    private void HandleReloading()
    {
        if (!isReloading) return;

        if (TimeServer.Elapsed - reloadStartTime >= reloadTime)
        {
            // Infinite ammo implementation as ordered
            bulletsInMagazine = magazineSize;
            isReloading = false;
            Console.WriteLine("Reload complete. Magazine full.");
        }
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