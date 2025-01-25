using Raylib_cs;

namespace Cherris.DrawCommands;

public class CircleDC : DrawCommand
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Radius { get; set; } = 0f;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        Raylib.DrawCircleV(
            Position,
            Radius,
            Color);
    }
}