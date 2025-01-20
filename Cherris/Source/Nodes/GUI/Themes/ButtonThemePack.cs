﻿namespace Cherris;

public sealed class ButtonThemePack
{
    // States

    public ButtonTheme Current { get; set; } = new();

    public ButtonTheme Normal { get; set; } = new();

    public ButtonTheme Hover { get; set; } = new()
    {
        FillColor = DefaultTheme.HoverFill
    };

    public ButtonTheme Pressed { get; set; } = new()
    {
        FillColor = DefaultTheme.Accent
    };

    public ButtonTheme Disabled { get; set; } = new()
    {
        FillColor = DefaultTheme.DisabledFill,
        BorderColor = DefaultTheme.DisabledBorder,
        FontColor = DefaultTheme.DisabledText
    };

    public ButtonTheme Focused { get; set; } = new()
    {
        BorderColor = DefaultTheme.FocusBorder,
        BorderLength = 1
    };

    // Setters

    public float FontSpacing
    {
        set
        {
            Normal.FontSpacing = value;
            Hover.FontSpacing = value;
            Pressed.FontSpacing = value;
            Focused.FontSpacing = value;
            Current.FontSpacing = value;
        }
    }

    public float FontSize
    {
        set
        {
            Normal.FontSize = value;
            Hover.FontSize = value;
            Pressed.FontSize = value;
            Focused.FontSize = value;
            Current.FontSize = value;
        }
    }

    public Font Font
    {
        set
        {
            Normal.Font = value;
            Hover.Font = value;
            Pressed.Font = value;
            Focused.Font = value;
            Current.Font = value;
        }
    }

    public Color FontColor
    {
        set
        {
            Normal.FontColor = value;
            Hover.FontColor = value;
            Pressed.FontColor = value;
            Focused.FontColor = value;
            Current.FontColor = value;
        }
    }

    public float Roundness
    {
        set
        {
            Current.Roundness = value;
            Normal.Roundness = value;
            Hover.Roundness = value;
            Pressed.Roundness = value;
            Focused.Roundness = value;
            Disabled.Roundness = value;
        }
    }

    public float BorderLength
    {
        set
        {
            Current.BorderLength = value;
            Normal.BorderLength = value;
            Hover.BorderLength = value;
            Pressed.BorderLength = value;
            Disabled.BorderLength = value;
            Focused.BorderLength = value;
        }
    }

    public Color FillColor
    {
        set
        {
            Current.FillColor = value;
            Normal.FillColor = value;
            Hover.FillColor = value;
            Pressed.FillColor = value;
            Disabled.FillColor = value;
            Focused.FillColor = value;
        }
    }

    public Color BorderColor
    {
        set
        {
            Current.BorderColor = value;
            Normal.BorderColor = value;
            Hover.BorderColor = value;
            Pressed.BorderColor = value;
            Disabled.BorderColor = value;
            Focused.BorderColor = value;
        }
    }

    public float BorderLengthTop
    {
        set
        {
            Current.BorderLengthTop = value;
            Normal.BorderLengthTop = value;
            Hover.BorderLengthTop = value;
            Pressed.BorderLengthTop = value;
            Disabled.BorderLengthTop = value;
            Focused.BorderLengthTop = value;
        }
    }

    public float BorderLengthBottom
    {
        set
        {
            Current.BorderLengthBottom = value;
            Normal.BorderLengthBottom = value;
            Hover.BorderLengthBottom = value;
            Pressed.BorderLengthBottom = value;
            Disabled.BorderLengthBottom = value;
            Focused.BorderLengthBottom = value;
        }
    }
}