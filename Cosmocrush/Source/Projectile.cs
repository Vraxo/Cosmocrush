using Cherris;
using Box2D.NetStandard.Dynamics.Bodies;

namespace Cosmocrush
{
    public class Projectile : Area2D
    {
        public float Speed { get; set; } = 500f;

        private Vector2 _direction = Vector2.Zero;
        public Vector2 Direction
        {
            get => _direction;
            set => _direction = value.Normalized(); // Ensure direction is normalized
        }

        public override void Ready()
        {
            base.Ready();
            BodyEntered += OnBodyEntered;

            // Set initial velocity when the projectile is ready
            if (Box2DBody != null)
            {
                var velocity = new Vector2(Direction.X * Speed, Direction.Y * Speed);
                Box2DBody.SetLinearVelocity(velocity);
            }
        }

        public override void Process()
        {
            base.Process();

            // No manual position updates; movement is now physics-driven
        }

        private void OnBodyEntered(Node otherBody)
        {
            if (otherBody is Player player)
            {
                Log.Info("Projectile hit Player!");
                player.TakeDamage(10); // Example: Deal 10 damage to the player
                Destroy();
            }
            else
            {
                Log.Info($"Projectile hit something else: {otherBody.Name}");
                Destroy();
            }
        }

        private void Destroy()
        {
            Free();
        }
    }
}