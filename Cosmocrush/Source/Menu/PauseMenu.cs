using Cherris;

namespace Cosmocrush;

public class PauseMenu : ColorRectangle
{
    private readonly Button? continueButton;
    private readonly Button? quitButton;

    public override void Ready()
    {
        base.Ready();

        InheritPosition = false;

        continueButton!.LeftClicked += OnContinueButtonLeftClicked;
        quitButton!.LeftClicked += OnQuitButtonLeftClicked;
    }

    public override void Process()
    {
        base.Process();

        Size = VisualServer.WindowSize;
        Position = RenderServer.Instance.GetScreenToWorld(VisualServer.WindowSize / 2);
    }

    private void OnContinueButtonLeftClicked(Button sender)
    {
        Tree.Paused = false;
        Free();
    }

    private void OnQuitButtonLeftClicked(Button sender)
    {
        Environment.Exit(0);
    }
}