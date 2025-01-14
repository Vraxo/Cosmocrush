﻿namespace Nodica;

public class LabelTheme
{
    public Font Font { get; set; } = FontManager.Instance.Get("RobotoMono 64");
    public Color FontColor { get; set; } = DefaultTheme.Text;
    public uint FontSize { get; set; } = 16;
    public int FontSpacing { get; set; } = 0;
    public bool EnableShadow { get; set; } = false;
    public Color ShadowColor { get; set; } = Color.Black;
    public Vector2 ShadowOffset { get; set; } = new Vector2(1);
}