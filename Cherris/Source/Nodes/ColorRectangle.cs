namespace Cherris;

public class ColorRectangle : Node2D
{
    static ColorRectangle()
    {
        PropertyRegistry.Register(typeof(ColorRectangle), new()
        {
            { "Color", (node, value) => ((ColorRectangle)node).Color = TypeParser.ParseColor(value) },
        });
    }

    public Color Color { get; set; } = Color.White;

    public ColorRectangle()
    {
        Size = new(32, 32);
        OriginPreset = OriginPreset.TopLeft;
    }

    protected override void Draw()
    {
        DrawRectangle(
            GlobalPosition - Origin,
            Size,
            Color);
    }
}