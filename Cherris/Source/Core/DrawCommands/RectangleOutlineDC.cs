using Raylib_cs;

namespace Cherris.DrawCommands;

public class RectangleOutlineDC : DrawCommand
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Size { get; set; } = Vector2.Zero;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        Raylib.DrawRectangleLines(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.X,
            (int)Size.Y,
            Color);
    }
}