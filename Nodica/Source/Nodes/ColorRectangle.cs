﻿namespace Nodica;

public class ColorRectangle : Node2D
{
    public Color Color { get; set; } = Color.White;

    public override void Ready()
    {
        base.Ready();

        Size = new(32, 32);
        OriginPreset = OriginPreset.TopLeft;
    }

    protected override void Draw()
    {
        DrawRectangle(
            GlobalPosition - Origin,
            Size,
            Color);
    }
}