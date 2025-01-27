﻿using Cherris;

namespace Cosmocrush;

public class MainMenu : Node
{
    private Button playButton = new();
    private Button settingsButton = new();
    private Button quitButton = new();
    private Label title = new();

    public override void Ready()
    {
        base.Ready();

        playButton = GetNode<Button>("PlayButton");
        playButton.LeftClicked += OnStartButtonLeftClicked;

        settingsButton = GetNode<Button>("SettingsButton");
        settingsButton.LeftClicked += OnSettingsButtonLeftClicked;

        quitButton = GetNode<Button>("QuitButton");
        quitButton.LeftClicked += OnQuitButtonLeftClicked;

        title = GetNode<Label>("Title");
    }

    public override void Update()
    {
        base.Update();

        UpdateTitle();
        UpdatePlayButton();
        UpdateSettingsButton();
        UpdateQuitButton();
    }

    private void OnStartButtonLeftClicked(Button sender)
    {
        PackedSceneIni packedMainScene = new("Res/Scenes/MainScene.ini");
        var mainScene = packedMainScene.Instantiate<Node>();
        ChangeScene(mainScene);
    }

    private void OnSettingsButtonLeftClicked(Button sender)
    {
        PackedSceneIni packedSettingsMenu = new("Res/Scenes/Menu/SettingsMenu.ini");
        var settingsMenu = packedSettingsMenu.Instantiate<Node2D>();
        GetParent<Menu>().AddChild(settingsMenu);

        Destroy();
    }

    private void OnQuitButtonLeftClicked(Button sender)
    {
        Environment.Exit(0);
    }

    private void UpdatePlayButton()
    {
        playButton.Position = WindowManager.Size / 2 - new Vector2(0, 50);
    }

    private void UpdateSettingsButton()
    {
        settingsButton.Position = WindowManager.Size / 2;
    }

    private void UpdateQuitButton()
    {
        quitButton.Position = WindowManager.Size / 2 + new Vector2(0, 50);
    }

    private void UpdateTitle()
    {
        title.Position = new(
            WindowManager.Size.X / 2,
            title.Position.Y);
    }
}