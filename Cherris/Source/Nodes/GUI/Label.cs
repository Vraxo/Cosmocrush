namespace Cherris;

public class Label : Control
{
    public enum TextCase
    {
        Upper,
        Lower,
        Both
    }

    public LabelStyle Style { get; set; } = new();
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

    public override void Process()
    {
        ClipDisplayedText();
        ApplyCase();

        base.Process();
    }

    protected override void OnThemeFileChanged(string themeFile)
    {
        Style = FileLoader.Load<LabelStyle>(themeFile);
    }

    // Drawing

    public override void Draw()
    {
        base.Draw();
        DrawShadow();
        DrawDisplayedText();
    }

    private void DrawShadow()
    {
        if (!Style.EnableShadow)
        {
            return;
        }

        DrawText(
            displayedText,
            GlobalPosition - Origin + Style.ShadowOffset - new Vector2(0, Style.FontSize / 2),
            Style.Font,
            Style.FontSize * Scale.Length(),
            Style.FontSpacing,
            Style.ShadowColor);
    }

    private void DrawDisplayedText()
    {
        DrawTextOutlined(
            displayedText,
            GlobalPosition - Origin - new Vector2(0, Style.FontSize / 2),
            Style.Font,
            Style.FontSize * Scale.Length(),
            Style.FontSpacing,
            Style.FontColor,
            Style.OutlineSize,
            Style.OutlineColor);
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

        int numFittingCharacters = (int)(Size.X / Style.Font.Dimensions.X + Style.FontSpacing);

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
            float scaledFontSize = Style.FontSize * Scale.Length();

            Vector2 textSize = Raylib_cs.Raylib.MeasureTextEx(
                Style.Font,
                displayedText,
                scaledFontSize,
                Style.FontSpacing);

            return new(textSize.X, textSize.Y);
        }
    }

    public new Vector2 Origin
    {
        get
        {
            float scaledFontSize = Style.FontSize * Scale.Length();
            Vector2 textSize = Raylib_cs.Raylib.MeasureTextEx(Style.Font, displayedText, scaledFontSize, Style.FontSpacing);

            float x = HAlignment switch
            {
                HAlignment.Center => textSize.X / 2,
                HAlignment.Left => 0,
                HAlignment.Right => textSize.X,
                _ => 0
            };

            float y = VAlignment switch
            {
                VAlignment.Top => 0,
                VAlignment.Center => textSize.Y / 2,
                VAlignment.Bottom => textSize.Y,
                _ => 0
            };

            Vector2 alignmentOffset = new(x, y);
            return alignmentOffset + Offset;
        }
    }
}