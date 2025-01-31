using Cherris;

namespace Cosmocrush;

public class Menu : Node
{
    public override void Ready()
    {
        base.Ready();
        GameSettings.Instance.Load();
    }
}