namespace Cherris;

public class ThemedRectangle : Node2D
{
    public BoxStyle Style = new();

    public ThemedRectangle()
    {
        Size = new(32, 32);
        OriginPreset = OriginPreset.TopLeft;
    }

    public override void Draw()
    {
        DrawRectangleThemed(
            GlobalPosition - Origin,
            Size,
            Style);
    }
}