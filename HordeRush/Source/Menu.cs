using Nodica;

namespace Cosmocrush;

public class Menu : Node
{
    private Button startButton = new();

    public override void Start()
    {
        base.Start();

        startButton = GetNode<Button>("StartButton");
        startButton.LeftClicked += OnStartButtonLeftClicked;
    }

    public override void Update()
    {
        base.Update();

        startButton.Position = Window.Size / 2;
    }

    private void OnStartButtonLeftClicked(object? sender, EventArgs e)
    {
        PackedScene packedMainScene = new("MainScene.scene");
        var mainScene = packedMainScene.Instantiate<MainScene>();
        ChangeScene(mainScene);
    }
}