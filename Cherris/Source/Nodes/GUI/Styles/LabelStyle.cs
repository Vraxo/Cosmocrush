﻿namespace Cherris;

public class LabelStyle
{
    public Font? Font { get; set; } = ResourceLoader.Load<Font>("Res/Cherris/RobotoMono.ttf:16");
    public Color FontColor { get; set; } = DefaultTheme.Text;
    public uint FontSize { get; set; } = 16;
    public int FontSpacing { get; set; } = 0;
    public bool EnableShadow { get; set; } = false;
    public Color ShadowColor { get; set; } = Color.Black;
    public Vector2 ShadowOffset { get; set; } = new(1, 1);
    public float OutlineSize { get; set; } = 0;
    public Color OutlineColor { get; set; } = Color.Black;
}