using Nodica;

namespace HordeRush;

public class Menu : Node
{
    private Button startButton = new();
    private ColorRectangle background = new();

    public override void Ready()
    {
        base.Ready();

        startButton = GetNode<Button>("StartButton");
        startButton.LeftClicked += OnStartButtonLeftClicked;

        background = GetNode<ColorRectangle>("Background");
    }

    public override void Update()
    {
        base.Update();

        GenerateParticle();

        UpdateBackground();
        UpdateStartButton();
    }

    private void OnStartButtonLeftClicked(object? sender, EventArgs e)
    {
        PackedScene packedMainScene = new("Res/Scenes/MainScene.scene");
        var mainScene = packedMainScene.Instantiate<MainScene>();
        ChangeScene(mainScene);
    }

    private void UpdateStartButton()
    {
        startButton.Position = Window.Size / 2;
    }

    private void UpdateBackground()
    {
        background.Position = Window.Size / 2;
        background.Size = Window.Size;
    }

    private void GenerateParticle()
    {
        Random random = new();

        float speed = (float)(random.NextDouble() * (2.0 - 0.5) + 0.5) * 100;

        float y = random.Next(0, (int)Window.Size.Y);

        PackedScene packedScene = new("Res/Scenes/Menu/MenuParticle.scene");
        var particle = packedScene.Instantiate<MenuParticle>();

        particle.GlobalPosition = new(0, y);
        particle.Speed = speed;

        AddChild(particle);
    }
}