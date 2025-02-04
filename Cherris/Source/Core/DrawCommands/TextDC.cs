using Raylib_cs;

namespace Cherris.DrawCommands;

public class TextDC : DrawCommand
{
    public string Text { get; set; } = "";
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Font Font { get; set; } = ResourceLoader.Load<Font>("Res/Cherris/Fonts/RobotoMono.ttf:16");
    public float FontSize { get; set; } = 0;
    public float Spacing { get; set; } = 0;
    public float OutlineSize { get; set; } = 0;
    public Color OutlineColor { get; set; } = Color.Black;
    public Color Color { get; set; } = Color.White;

    public override void Draw()
    {
        if (OutlineSize > 0)
        {
            for (int x = (int)-OutlineSize; x <= OutlineSize; x++)
            {
                for (int y = (int)-OutlineSize; y <= OutlineSize; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Raylib.DrawTextEx(
                        Font,
                        Text,
                        Position + new Vector2(x, y),
                        FontSize,
                        Spacing,
                        OutlineColor
                    );
                }
            }
        }

        Raylib.DrawTextEx(
            Font,
            Text,
            Position,
            FontSize,
            Spacing,
            Color
        );
    }
}