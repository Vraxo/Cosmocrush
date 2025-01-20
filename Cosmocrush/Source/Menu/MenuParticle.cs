using Cherris;

namespace Cosmocrush;

public class MenuParticle : ColorRectangle
{
    public static readonly float BaseSpeed = 100;

    public float Speed = BaseSpeed;

    public override void Ready()
    {
        base.Ready();

        Size = new(1, 1);

        GetNode<Timer>("DestructionTimer").TimedOut += OnDestructionTimerTimedOut;
    }

    private void OnDestructionTimerTimedOut(Timer sender)
    {
        Destroy();
    }

    public override void Update()
    {
        base.Update();

        float x = GlobalPosition.X + Speed * Time.Delta;
        float y = GlobalPosition.Y;

        GlobalPosition = new(x, y);
    }
}