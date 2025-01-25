using Raylib_cs;

namespace Cherris.DrawCommands;

public class TexturedRectangleDC : DrawCommand
{
    public Texture Texture { get; set; } = new();
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public Vector2 Scale { get; set; } = Vector2.One;
    public Rectangle Source { get; set; } = new();
    public Rectangle Target { get; set; } = new();

    public override void Draw()
    {
        Raylib.DrawTexturePro(
            Texture,
            Source,
            Target,
            Origin,
            Rotation,
            Color.White
        );
    }
}
