using Raylib_cs;

namespace Cherris.DrawCommands;

public class TextDC : DrawCommand
{
    public string Text { get; set; } = "";
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Font Font { get; set; } = ResourceLoader.Load<Font>("Res/Cherris/Fonts/RobotoMono.ttf:16");
    public float FontSize { get; set; } = 0;
    public float Spacing { get; set; } = 0;
    public float OutlineThickness { get; set; } = 0;
    public Color OutlineColor { get; set; } = Color.Black;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        if (OutlineThickness > 0)
        {
            // Draw the outline by offsetting in eight directions
            Vector2[] offsets = [
                new(-OutlineThickness, -OutlineThickness),
                new(OutlineThickness, -OutlineThickness),
                new(-OutlineThickness, OutlineThickness),
                new(OutlineThickness, OutlineThickness),
                new(-OutlineThickness, 0),
                new(OutlineThickness, 0),
                new(0, -OutlineThickness),
                new(0, OutlineThickness),
            ];

            foreach (Vector2 offset in offsets)
            {
                Raylib.DrawTextEx(
                    Font,
                    Text,
                    Position + offset,
                    FontSize,
                    Spacing,
                    OutlineColor);
            }
        }

        // Draw the main text
        Raylib.DrawTextEx(
            Font,
            Text,
            Position,
            FontSize,
            Spacing,
            Color);
    }
}