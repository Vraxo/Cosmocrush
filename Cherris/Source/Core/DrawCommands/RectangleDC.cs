using Raylib_cs;

namespace Cherris.DrawCommands;

public class RectangleDC : DrawCommand
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Size { get; set; } = Vector2.Zero;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        Raylib.DrawRectangleV(
            Position,
            Size,
            Color);
    }
}