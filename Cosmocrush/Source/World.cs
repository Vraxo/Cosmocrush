using Cherris;

namespace Cosmocrush;

public class World : Node
{
    private ColorRectangle background = new();
    private readonly PackedScene pauseMenuScene = new("Res/Scenes/Menu/PauseMenu.yaml");
    private PauseMenu pauseMenu;

    public override void Ready()
    {
        base.Ready();

        //ProcessingMode = ProcessMode.Always;
        background = GetNode<ColorRectangle>("Background");
    }

    public override void Process()
    {
        base.Process();

        background.Position = VisualServer.WindowSize / 2;

        if (Input.IsKeyPressed(KeyCode.Space))
        {
            if (!Tree.Paused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    private void Pause()
    {
        Tree.Paused = true;

        var pauseMenuNode = pauseMenuScene.Instantiate<PauseMenu>();
        pauseMenu = (PauseMenu)AddChild(pauseMenuNode);
    }

    private void Resume()
    {
        Tree.Paused = false;
        pauseMenu.Free();
    }
}