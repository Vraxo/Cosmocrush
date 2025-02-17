using Cherris;

namespace Cosmocrush;

public class MainCamera : Camera
{
    public override void Ready()
    {
        base.Ready();

        SetAsActive();
    }
}