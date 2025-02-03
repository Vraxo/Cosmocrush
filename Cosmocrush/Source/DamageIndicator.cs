using Cherris;

namespace Cosmocrush;

public class DamageIndicator : Label
{
    private readonly float speed = 100;

    public override void Ready()
    {
        base.Ready();

        InheritScale = false;

        Scale = new(2);
    }

    public override void Update()
    {
        base.Update();

        Position = new(
            Position.X,
            Position.Y - speed * TimeManager.Delta);
    }
}