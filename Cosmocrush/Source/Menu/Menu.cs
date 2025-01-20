﻿using Cherris;

namespace Cosmocrush;

public class Menu : Node
{
    private Button startButton = new();
    private Button quitButton = new();
    private TextureRectangle background = new();
    private Label title = new();
    private float particleSpawnTimer = 0f;
    private const float ParticleSpawnInterval = 0.1f; // Spawn 10 particles per second
    private readonly Random randomInstance = new(); // Reuse Random instance

    public override void Ready()
    {
        base.Ready();

        startButton = GetNode<Button>("StartButton");
        startButton.LeftClicked += OnStartButtonLeftClicked;

        quitButton = GetNode<Button>("QuitButton");
        quitButton.LeftClicked += OnQuitButtonLeftClicked;

        background = GetNode<TextureRectangle>("TrueBackground");
        title = GetNode<Label>("Title");
    }

    public override void Update()
    {
        base.Update();

        particleSpawnTimer += Time.Delta;
        if (particleSpawnTimer >= ParticleSpawnInterval)
        {
            GenerateParticle();
            particleSpawnTimer = 0f;
        }

        UpdateTitle();
        UpdateBackground();
        UpdateStartButton();
        UpdateQuitButton();
    }

    private void OnStartButtonLeftClicked(Button sender)
    {
        PackedSceneIni packedMainScene = new("Res/Scenes/MainScene.ini");
        var mainScene = packedMainScene.Instantiate<Node>();
        ChangeScene(mainScene);
    }

    private void OnQuitButtonLeftClicked(Button sender)
    {
        Environment.Exit(0);
    }

    private void UpdateStartButton()
    {
        startButton.Position = WindowManager.Size / 2 - new Vector2(0, 50);
    }

    private void UpdateQuitButton()
    {
        quitButton.Position = WindowManager.Size / 2 + new Vector2(0, 50);
    }

    private void UpdateBackground()
    {
        background.Position = WindowManager.Size / 2;
        background.Size = WindowManager.Size;
    }

    private void UpdateTitle()
    {
        title.Position = new(WindowManager.Size.X / 2, title.Position.Y);
    }

    private void GenerateParticle()
    {
        float speed = (float)(randomInstance.NextDouble() * (2.0 - 0.5) + 0.5) * 100;
        float y = randomInstance.Next(0, (int)WindowManager.Size.Y);
        
        // Add variation to starting position
        float x = randomInstance.Next(-50, 0); // Start slightly off-screen

        PackedSceneIni packedScene = new("Res/Scenes/Menu/MenuParticle.ini");
        var particle = packedScene.Instantiate<MenuParticle>();

        particle.GlobalPosition = new(x, y);
        particle.Speed = speed;

        AddChild(particle);
    }
}