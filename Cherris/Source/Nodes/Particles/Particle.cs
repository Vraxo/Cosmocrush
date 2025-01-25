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
            GetNode<Timer>("DestructionTimer").TimedOut += OnDestructionTimerTimedOut;
            GetNode<Timer>("DestructionTimer").WaitTime = Lifetime;
        }

        private void OnDestructionTimerTimedOut(Timer sender)
        {
            GetParent<ParticleGenerator>().Remove(this);
            Destroy();
        }

        public override void Update()
        {
            base.Update();

            // Update speed with acceleration
            Speed += Acceleration * TimeManager.Delta;

            // Update position
            float x = Position.X + Speed * TimeManager.Delta;
            float y = Position.Y;

            Position = new(x, y);
        }
    }
}