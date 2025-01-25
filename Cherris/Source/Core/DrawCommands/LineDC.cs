using Raylib_cs;

namespace Cherris.DrawCommands;

public class LineDC : DrawCommand
{
    public Vector2 Start { get; set; } = Vector2.Zero;
    public Vector2 End { get; set; } = Vector2.Zero;
    public float Thickness { get; set; } = 1;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        Raylib.DrawLineEx(
            Start,
            End,
            Thickness,
            Color);
    }
}