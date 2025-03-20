using Cherris;

namespace Cosmocrush;

public class World : Node2D
{
    private ColorRectangle background = new();
    private readonly PackedScene pauseMenuScene = new("Res/Scenes/Menu/PauseMenu.yaml");

    public override void Ready()
    {
        base.Ready();

        background = GetNode<ColorRectangle>("Background");
    }

    public override void Process()
    {
        base.Process();

        background.Position = DisplayServer.WindowSize / 2;

        if (Input.IsKeyPressed(KeyCode.Space) && !Tree.Paused)
        {
            Pause();
        }
    }

    private void Pause()
    {
        Tree.Paused = true;

        var pauseMenuNode = pauseMenuScene.Instantiate<PauseMenu>();
        AddChild(pauseMenuNode);
    }
}