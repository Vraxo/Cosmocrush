using Cherris;

namespace Cosmocrush;

public class RangedEnemy : BaseEnemy
{
    protected override float ProximityThreshold => 320f;
    protected override float DamageRadius => ProximityThreshold;

    protected override void AttemptToDamagePlayer()
    {
        bool isPlayerInRange = GlobalPosition.DistanceTo(player!.GlobalPosition) <= DamageRadius;

        if (!isPlayerInRange || !canShoot)
        {
            return;
        }

        PackedScene projectileScene = new("Res/Scenes/Projectile.yaml");
        var projectile = projectileScene.Instantiate<Projectile>();
        projectile.Position = GlobalPosition;
        projectile.Direction = GlobalPosition.AngleTo(player.GlobalPosition);

        Tree.RootNode!.AddChild(projectile);

        canShoot = false;
        damageTimer!.Fire();
    }
}