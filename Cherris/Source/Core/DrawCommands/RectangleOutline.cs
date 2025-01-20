namespace Cherris.DrawCommands;

public class RectangleOutline : DrawCommand
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Size { get; set; } = Vector2.Zero;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        App.Instance.Backend.Drawing.DrawRectangleOutline(Position, Size, Color);
    }
}