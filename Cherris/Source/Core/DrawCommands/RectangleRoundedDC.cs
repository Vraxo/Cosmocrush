using Raylib_cs;

namespace Cherris.DrawCommands;

public class RectangleRoundedDC : DrawCommand
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Size { get; set; } = Vector2.Zero;
    public float Roundness { get; set; } = 0.5f;
    public int Segments { get; set; } = 0;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        Rectangle rectangle = new()
        {
            Position = Position,
            Size = Size
        };

        Raylib.DrawRectangleRounded(
            rectangle,
            Roundness,
            Segments,
            Color);
    }
}