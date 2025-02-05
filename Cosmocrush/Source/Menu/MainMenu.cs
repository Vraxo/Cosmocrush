using Cherris;

namespace Cosmocrush;

public class MainMenu : Node
{
    private readonly Button? playButton;
    private readonly Button? settingsButton;
    private readonly Button? quitButton;
    private readonly Label? title;

    public override void Ready()
    {
        base.Ready();

        playButton!.LeftClicked += OnPlayButtonLeftClicked;
        settingsButton!.LeftClicked += OnSettingsButtonLeftClicked;
        quitButton!.LeftClicked += OnQuitButtonLeftClicked;
    }

    public override void Update()
    {
        base.Update();

        UpdateTitle();
        UpdatePlayButton();
        UpdateSettingsButton();
        UpdateQuitButton();
    }

    private void OnPlayButtonLeftClicked(Button sender)
    {
        PackedScene packedMainScene = new("Res/Scenes/World.yaml");
        var mainScene = packedMainScene.Instantiate<Node>();
        ChangeScene(mainScene);
    }

    private void OnSettingsButtonLeftClicked(Button sender)
    {
        PackedScene packedSettingsMenu = new("Res/Scenes/Menu/SettingsMenu.yaml");
        var settingsMenu = packedSettingsMenu.Instantiate<Node2D>();
        GetParent<Menu>().AddChild(settingsMenu);

        Free();
    }

    private void OnQuitButtonLeftClicked(Button sender)
    {
        Environment.Exit(0);
    }

    private void UpdatePlayButton()
    {
        playButton!.Position = VisualServer.WindowSize / 2 - new Vector2(0, 50);
    }

    private void UpdateSettingsButton()
    {
        settingsButton!.Position = VisualServer.WindowSize / 2;
    }

    private void UpdateQuitButton()
    {
        quitButton!.Position = VisualServer.WindowSize / 2 + new Vector2(0, 50);
    }

    private void UpdateTitle()
    {
        title!.Position = new(
            VisualServer.WindowSize.X / 2,
            title.Position.Y);
    }
}