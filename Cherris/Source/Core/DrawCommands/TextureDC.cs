using Raylib_cs;

namespace Cherris.DrawCommands;

public class TextureDC : DrawCommand
{
    public Texture Texture { get; set; } = new();
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public Vector2 Scale { get; set; } = Vector2.One;
    public Color Tint { get; set; } = Color.White;

    public override void Draw()
    {
        Raylib.DrawTextureEx(
            Texture,
            Position,
            Rotation,
            Scale.Length(),
            Tint);
    }
}