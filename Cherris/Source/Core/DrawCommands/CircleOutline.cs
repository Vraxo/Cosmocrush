﻿namespace Cherris.DrawCommands;

public class CirlceOutline : DrawCommand
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Radius { get; set; } = 0f;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        App.Instance.Backend.Drawing.DrawCircleOutline(Position, Radius, Color);
    }
}