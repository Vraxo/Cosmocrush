namespace Cherris;

public class ButtonTheme : BoxStyle
{
    public float FontSpacing { get; set; } = 0;
    public float FontSize { get; set; } = 16;
    public Font Font { get; set; } = FontCache.Instance.Get("Res/Cherris/Fonts/RobotoMono.ttf:16");
    public Color FontColor { get; set; } = DefaultTheme.Text;
}