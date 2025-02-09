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
    private readonly Line? bulletTrail;
    private ProgressBar? reloadProgressBar;
    
    private bool canFire = true;
    private bool reloading = false;
    private int bulletsInMagazine = MagazineSize;
    private float currentBloom = 0f;

    private const int Damage = 5;
    private const int MagazineSize = 100;
    private const float KnockbackForce = 3f;
    private const float MaxBloom = 0.0f;
    private const float BloomIncrease = 0.02f;
    private const float BloomResetSpeed = 0.05f;

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

        if (!Input.IsActionDown("Fire"))
        {
            currentBloom = float.Max(0, currentBloom - BloomResetSpeed * TimeServer.Delta);
        }
    }

    // Timers

    private void OnCooldownTimerTimeout(Timer timer)
    {
        canFire = true;
    }

    private void OnReloadTimerTimeout(Timer timer)
    {
        bulletsInMagazine = MagazineSize;
        reloading = false;
        Console.WriteLine("Reload complete. Magazine full.");
        RemoveReloadProgressBar();
    }

    // Reloading

    private void HandleReloadingInput()
    {
        if (Input.IsActionPressed("Reload") && !reloading && bulletsInMagazine < MagazineSize)
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

    // Firing

    private void HandleFiring()
    {
        bool canShoot = canFire && !reloading && bulletsInMagazine > 0;
        if (Input.IsActionDown("Fire") && canShoot)
        {
            Fire();
        }
    }

    private void Fire()
    {
        canFire = false;
        cooldownTimer!.Fire();
        bulletsInMagazine--;
        gunshotSound.Play("SFX");
        FireRayCast();
        UpdateBulletTrail();
        currentBloom = float.Min(MaxBloom, currentBloom + BloomIncrease);
        if (bulletsInMagazine <= 0)
        {
            StartReloading();
        }
    }

    private void UpdateBulletTrail()
    {
        bulletTrail!.ClearPoints();
        bulletTrail.AddPoint(bulletTrail.GlobalPosition);
        Vector2 endPoint = rayCast!.IsColliding ? rayCast.CollisionPoint : bulletTrail.GlobalPosition + new Vector2(MathF.Cos(rayCast.Rotation * MathF.PI / 180f), MathF.Sin(rayCast.Rotation * MathF.PI / 180f)) * 10000f;
        bulletTrail.AddPoint(endPoint);
        bulletTrail.Visible = true;
        Tree.CreateTimer(0.025f).Timeout += () => bulletTrail.Visible = false;
    }

    private void LookAtMouse()
    {
        FlipV = Input.WorldMousePosition.X < GlobalPosition.X;
        LookAt(Input.WorldMousePosition);
    }

    private void FireRayCast()
    {
        Vector2 mousePosition = Input.WorldMousePosition;
        Vector2 angleVector = mousePosition - GlobalPosition;

        var angle = float.Atan2(angleVector.Y, angleVector.X);
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
                    enemy.TakeDamage(Damage);
                    enemy.ApplyKnockback(angleVector.Normalized() * KnockbackForce);
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

    // Utils

    private static float RandomRange(float min, float max)
    {
        return (float)(new Random().NextDouble() * (max - min) + min);
    }
}