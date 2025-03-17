namespace Cherris;

public class CheckBoxStylePack
{
    // States

    public BoxStyle Current { get; set; } = new();

    public BoxStyle Normal { get; set; } = new();

    public BoxStyle Hover { get; set; } = new()
    {
        FillColor = DefaultTheme.HoverFill
    };

    public BoxStyle Pressed { get; set; } = new()
    {
        FillColor = DefaultTheme.Accent
    };

    public BoxStyle Disabled { get; set; } = new()
    {
        FillColor = DefaultTheme.DisabledFill,
        BorderColor = DefaultTheme.DisabledBorder,
    };

    public BoxStyle Focused { get; set; } = new()
    {
        BorderColor = DefaultTheme.FocusBorder,
        BorderLength = 1
    };

    // Setters
    
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

    public float OutlineThickness
    {
        set
        {
            Current.BorderLength = value;
            Normal.BorderLength = value;
            Hover.BorderLength = value;
            Pressed.BorderLength = value;
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
        }
    }

    public float BorderLengthUp
    {
        set
        {
            Current.BorderLengthTop = value;
            Normal.BorderLengthTop = value;
            Hover.BorderLengthTop = value;
            Pressed.BorderLengthTop = value;
        }
    }
}