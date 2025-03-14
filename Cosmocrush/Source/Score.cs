using Cherris;

namespace Cosmocrush;

public sealed class Score : Label
{
    public override void Ready()
    {
        base.Ready();

        InheritPosition = false;
    }

    public override void Process()
    {
        base.Process();

        GlobalPosition = new Vector2(25, 25) + RenderServer.Instance.Camera.GlobalPosition;
    }
}