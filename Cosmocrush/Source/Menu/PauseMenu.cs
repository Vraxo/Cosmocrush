using Cherris;

namespace Cosmocrush;

public class PauseMenu : ColorRectangle
{
    public override void Ready()
    {
        base.Ready();

        InheritPosition = false;
    }

    public override void Process()
    {
        base.Process();

        Size = VisualServer.WindowSize;
        //Position = VisualServer.WindowSize / 2; 
        Position = RenderServer.Instance.GetScreenToWorld(VisualServer.WindowSize / 2); 
    }
}