namespace Cherris;

public partial class ParticleGenerator
{
    private class Particle : ColorRectangle
    {
        public float Speed { get; set; } = 100;
        public float Lifetime { get; set; } = 10;
        public float Acceleration { get; set; } = 0; // Optional acceleration

        public override void Ready()
        {
            base.Ready();

            AddChild(new Timer(), "DestructionTimer");
            GetNode<Timer>("DestructionTimer").Timeout += OnDestructionTimerTimedOut;
            GetNode<Timer>("DestructionTimer").WaitTime = Lifetime;
        }

        private void OnDestructionTimerTimedOut(Timer sender)
        {
            GetParent<ParticleGenerator>().Remove(this);
            Free();
        }

        public override void Update()
        {
            base.Update();

            // Update speed with acceleration
            Speed += Acceleration * TimeServer.Delta;

            // Update position
            float x = Position.X + Speed * TimeServer.Delta;
            float y = Position.Y;

            Position = new(x, y);
        }
    }
}