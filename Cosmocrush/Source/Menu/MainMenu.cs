using Cherris;

namespace Cosmocrush;

public class MainMenu : Node
{
    private readonly Label? title;
    private readonly VBoxContainer? buttonContainer;
    private readonly Button? playButton;
    private readonly Button? settingsButton;
    private readonly Button? quitButton;

    // Main

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
        UpdateButtonContainer();
    }

    // Events

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

    // Update

    private void UpdateTitle()
    {
        title!.Position = new(
            VisualServer.WindowSize.X / 2,
            title.Position.Y);
    }

    private void UpdateButtonContainer()
    {
        buttonContainer!.Position = VisualServer.WindowSize / 2;
    }
}