﻿using Raylib_cs;

namespace Cherris;

public class TextureRectangle : Node2D
{
    public Texture? Texture { get; set; }

    public TextureRectangle()
    {
        Size = new(32, 32);
    }

    protected override void Draw()
    {
        if (Texture is null)
        {
            return;
        }

        DrawTextureScaled(
            Texture,
            GlobalPosition,
            Origin,
            0,
            Scale,
            false,
            false);

        Rectangle source = new(Vector2.Zero, Texture.Size);
        Rectangle target = new(GlobalPosition, ScaledSize);

        DrawTexturedRectangle(
            Texture,
            source,
            target,
            Origin,
            Rotation
        );
    }
}