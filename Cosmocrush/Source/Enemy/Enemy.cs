using Cherris;

namespace Cosmocrush;

public class Enemy : BaseEnemy
{
    protected override void AttemptToDamagePlayer()
    {
        bool isPlayerInRange = GlobalPosition.DistanceTo(player!.GlobalPosition) <= DamageRadius;

        if (!isPlayerInRange || !canShoot)
        {
            return;
        }

        player?.TakeDamage(Damage);

        Vector2 knockbackDirection = (GlobalPosition - player!.GlobalPosition).Normalized();
        player.ApplyKnockback(knockbackDirection * KnockbackForce);

        canShoot = false;
        damageTimer!.Fire();
    }
}