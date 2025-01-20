using Raylib_cs;

namespace Cherris.DrawCommands;

public class Pixel : DrawCommand
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        Raylib.DrawPixelV(
            Position,
            Color);
    }
}