using Cherris;

namespace Cosmocrush;

public class MainCamera : Camera
{
    public override void Ready()
    {
        base.Ready();

        SetAsActive();
    }

    public override void Process()
    {
        base.Process();

        Size = DisplayServer.WindowSize;
    }
}