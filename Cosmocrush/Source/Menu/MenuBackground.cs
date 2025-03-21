﻿using Cherris;

namespace Cosmocrush;

public class MenuBackground : TextureRectangle
{
    private readonly string pathTemplate = "Res/Sprites/MenuBackgrounds/MenuBackground{0}.png";
    private float oscillationSpeed = 1.0f;
    private float amplitude = 20.0f;

    public override void Ready()
    {
        base.Ready();

        Random random = new();
        string path = string.Format(pathTemplate, random.Next(1, 7)); // Generates a number in the range [1, 6]
        Texture = ResourceLoader.Load<Texture>(path);
    }

    public override void Process()
    {
        base.Process();

        Size = DisplayServer.WindowSize * 1.5f;

        // Calculate the center position of the window
        Vector2 windowCenter = DisplayServer.WindowSize / 2;

        // Calculate oscillating offsets
        float offsetX = amplitude * MathF.Sin(Time.Elapsed * oscillationSpeed);
        float offsetY = amplitude * MathF.Cos(Time.Elapsed * oscillationSpeed * 0.8f); // Slightly slower Y movement

        // Process the position to stay centered with oscillation
        Position = windowCenter + new Vector2(offsetX, offsetY);
    }
}