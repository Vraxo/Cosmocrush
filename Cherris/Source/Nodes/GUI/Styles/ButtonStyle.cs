namespace Cherris;

public class ButtonStyle : BoxStyle
{
    public float FontSpacing { get; set; } = 0;
    public float FontSize { get; set; } = 16;
    public Font? Font { get; set; } = ResourceLoader.Load<Font>("Res/Cherris/Fonts/RobotoMono.ttf:16");
    public Color FontColor { get; set; } = DefaultTheme.Text;
}