﻿namespace Nodica;

public class Label : Node2D
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

    private int _visibleCharacters = -1;
    public int VisibleCharacters
    {
        get => _visibleCharacters;

        set
        {
            if (value == _visibleCharacters)
            {
                return;
            }

            _visibleCharacters = value;
            UpdateVisibleRatio();
        }
    }

    private float _visibleRatio = 1.0f;
    public float VisibleRatio
    {
        get => _visibleRatio;

        set
        {
            if (value == _visibleRatio)
            {
                return;
            }

            _visibleRatio = value;
            UpdateVisibleCharacters();
        }
    }

    private string displayedText = "";

    private string _text = "";
    public string Text
    {
        get => _text;

        set
        {
            _text = value;
            UpdateVisibleCharacters();
        }
    }

    public Label()
    {
        OriginPreset = OriginPreset.CenterLeft;
    }

    public override void Update()
    {
        ClipDisplayedText();
        ApplyCase();
        base.Update();
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
            GlobalPosition - Origin + Theme.ShadowOffset,
            Theme.Font,
            Theme.FontSize,
            Theme.FontSpacing,
            Theme.ShadowColor);
    }

    private void DrawDisplayedText()
    {
        DrawText(
            displayedText,
            GlobalPosition - Offset - new Vector2(0, Theme.FontSize / 2),
            Theme.Font,
            Theme.FontSize,
            Theme.FontSpacing,
            Theme.FontColor);
    }

    // Text modification

    private void ClipDisplayedText()
    {
        string textToConsider = VisibleCharacters == -1 ? 
                                _text : 
                                _text[..Math.Min(VisibleCharacters, _text.Length)];

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
}