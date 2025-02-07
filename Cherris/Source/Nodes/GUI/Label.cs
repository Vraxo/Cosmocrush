namespace Cherris;

public class Label : Control
{
    public enum TextCase
    {
        Upper,
        Lower,
        Both
    }

    public LabelTheme Theme { get; set; } = new();
    public bool Clip { get; set; } = false;
    public string Ellipsis { get; set; } = "...";
    public TextCase Case { get; set; } = TextCase.Both;

    private string displayedText = "";

    public int VisibleCharacters
    {
        get;

        set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            UpdateVisibleRatio();
        }
    } = -1;

    public float VisibleRatio
    {
        get;

        set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            UpdateVisibleCharacters();
        }
    } = 1.0f;

    public string Text
    {
        get;
        set
        {
            field = value;
            UpdateVisibleCharacters();
        }
    } = "";

    // Main

    public Label()
    {

    }

    public override void Update()
    {
        ClipDisplayedText();
        ApplyCase();

        base.Update();
    }

    protected override void OnThemeFileChanged(string themeFile)
    {
        Theme = FileLoader.Load<LabelTheme>(themeFile);
    }

    // Drawing

    protected override void Draw()
    {
        DrawShadow();
        DrawDisplayedText();
    }

    private void DrawShadow()
    {
        if (!Theme.EnableShadow)
        {
            return;
        }

        DrawText(
            displayedText,
            GlobalPosition - Origin + Theme.ShadowOffset - new Vector2(0, Theme.FontSize / 2),
            Theme.Font,
            Theme.FontSize * Scale.Length(),
            Theme.FontSpacing,
            Theme.ShadowColor);
    }

    private void DrawDisplayedText()
    {
        DrawTextOutlined(
            displayedText,
            GlobalPosition - Origin - new Vector2(0, Theme.FontSize / 2),
            Theme.Font,
            Theme.FontSize * Scale.Length(),
            Theme.FontSpacing,
            Theme.FontColor,
            Theme.OutlineSize,
            Theme.OutlineColor);
    }

    // Text modification

    private void ClipDisplayedText()
    {
        string textToConsider = VisibleCharacters == -1 ?
                                Text :
                                Text[..Math.Min(VisibleCharacters, Text.Length)];

        if (!Clip)
        {
            displayedText = textToConsider;
            return;
        }

        int numFittingCharacters = (int)(Size.X / Theme.Font.Dimensions.X + Theme.FontSpacing);

        if (VisibleCharacters != -1)
        {
            numFittingCharacters = Math.Min(numFittingCharacters, VisibleCharacters);
        }

        if (numFittingCharacters <= 0)
        {
            displayedText = "";
        }
        else if (numFittingCharacters < textToConsider.Length)
        {
            string trimmedText = textToConsider[..numFittingCharacters];
            displayedText = ClipTextWithEllipsis(trimmedText);
        }
        else
        {
            displayedText = textToConsider;
        }
    }

    private string ClipTextWithEllipsis(string input)
    {
        if (input.Length > Ellipsis.Length)
        {
            string trimmedText = input[..^Ellipsis.Length];
            return trimmedText + Ellipsis;
        }
        else
        {
            return input;
        }
    }

    private void ApplyCase()
    {
        displayedText = Case switch
        {
            TextCase.Upper => displayedText.ToUpper(),
            TextCase.Lower => displayedText.ToLower(),
            _ => displayedText
        };
    }

    // Visibility control

    private void UpdateVisibleCharacters()
    {
        if (Text.Length > 0)
        {
            VisibleCharacters = (int)(Text.Length * VisibleRatio);
        }
        else
        {
            VisibleCharacters = -1; // Show all characters by default
        }
    }

    private void UpdateVisibleRatio()
    {
        if (Text.Length > 0)
        {
            VisibleRatio = VisibleCharacters / (float)Text.Length;
        }
        else
        {
            VisibleRatio = 1.0f; // Show all characters by default
        }
    }

    // Calculate the size based on the text

    public override Vector2 Size
    {
        get
        {
            float scaledFontSize = Theme.FontSize * Scale.Length();

            Vector2 textSize = Raylib_cs.Raylib.MeasureTextEx(
                Theme.Font,
                displayedText,
                scaledFontSize,
                Theme.FontSpacing);

            return new(textSize.X, textSize.Y);
        }
    }

    public new Vector2 Origin
    {
        get
        {
            float scaledFontSize = Theme.FontSize * Scale.Length();
            Vector2 textSize = Raylib_cs.Raylib.MeasureTextEx(Theme.Font, displayedText, scaledFontSize, Theme.FontSpacing);

            float x = HorizontalAlignment switch
            {
                HorizontalAlignment.Center => textSize.X / 2,
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Right => textSize.X,
                _ => 0
            };

            float y = VerticalAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Center => textSize.Y / 2,
                VerticalAlignment.Bottom => textSize.Y,
                _ => 0
            };

            Vector2 alignmentOffset = new(x, y);
            return alignmentOffset + Offset;
        }
    }
}