﻿namespace Nodica;

public class ButtonTheme : BoxTheme
{
    public float FontSpacing { get; set; } = 0;
    public float FontSize { get; set; } = 16;
    public Font Font { get; set; } = FontManager.Instance.Get("RobotoMono 16");
    public Color FontColor { get; set; } = DefaultTheme.Text;
}