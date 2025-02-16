﻿namespace Cherris;

public class Button : Control
{
    public enum ClickMode { Limited, Limitless }
    public enum ActionMode { Release, Press }
    public enum ClickBehavior { Left, Right, Both }

    #region [ - - - Properties & Fields - - - ]

    public Vector2 TextOffset { get; set; } = Vector2.Zero;
    public Alignment TextAlignment { get; set; } = new();
    public ButtonThemePack Themes { get; set; } = new();
    public float AvailableWidth { get; set; } = 0;
    public ActionMode LeftClickActionMode { get; set; } = ActionMode.Release;
    public ActionMode RightClickActionMode { get; set; } = ActionMode.Release;
    public bool StayPressed { get; set; } = false;
    public bool ClipText { get; set; } = false;
    public bool AutoWidth { get; set; } = false;
    public Vector2 TextMargin { get; set; } = new(10, 5);
    public string Ellipsis { get; set; } = "...";
    public ClickBehavior Behavior { get; set; } = ClickBehavior.Left;
    public Texture? Icon { get; set; } = null;
    public float IconMargin { get; set; } = 12;

    private bool pressedLeft = false;
    private bool pressedRight = false;

    public Action<Button> OnUpdate = (button) => { };

    public delegate void ButtonEventHandler(Button sender);
    public event ButtonEventHandler? LeftClicked;
    public event ButtonEventHandler? RightClicked;

    public Sound? ClickSound { get; set; }

    private string displayedText = "";

    private string _text = "";
    public string Text
    {
        get => _text;

        set
        {
            _text = value;
            displayedText = value;

            if (AutoWidth)
            {
                ResizeToFitText();
            }
        }
    }

    #endregion

    public Button()
    {
        Size = new(100, 26);
        Offset = new(0, 0);
        OriginPreset = OriginPreset.None;

        WasDisabled += (button) => Themes.Current = Disabled ? Themes.Disabled : Themes.Normal;
    }

    public override void Process()
    {
        base.Process();

        if (!Disabled)
        {
            OnUpdate(this);
            ClipDisplayedText();
            HandleClicks();
            HandleKeyboardInput();
            ResizeToFitText();
        }
    }

    protected virtual void OnEnterPressed() { }

    private void HandleKeyboardInput()
    {
        if (Focused && Input.IsKeyPressed(KeyCode.Enter))
        {
            if (Behavior == ClickBehavior.Left || Behavior == ClickBehavior.Both)
            {
                LeftClicked?.Invoke(this);
                OnEnterPressed();
            }

            if (Behavior == ClickBehavior.Right || Behavior == ClickBehavior.Both)
            {
                RightClicked?.Invoke(this);
                OnEnterPressed();
            }
        }
    }

    private void HandleClicks()
    {
        bool isMouseOver = IsMouseOver();
        bool isAnyPressed = false;

        if (Behavior == ClickBehavior.Left || Behavior == ClickBehavior.Both)
        {
            HandleClick(
                ref pressedLeft,
                ref OnTopLeft,
                MouseButtonCode.Left,
                LeftClickActionMode,
                LeftClicked);

            if (pressedLeft) isAnyPressed = true;
        }

        if (Behavior == ClickBehavior.Right || Behavior == ClickBehavior.Both)
        {
            HandleClick(
                ref pressedRight,
                ref OnTopRight,
                MouseButtonCode.Right,
                RightClickActionMode,
                RightClicked);

            if (pressedRight) isAnyPressed = true;
        }

        UpdateTheme(isMouseOver, isAnyPressed);
    }

    private void HandleClick(ref bool pressed, ref bool onTop, MouseButtonCode button, ActionMode actionMode, ButtonEventHandler? clickHandler)
    {
        if (Disabled)
        {
            return;
        }

        bool mouseOver = IsMouseOver();

        if (mouseOver)
        {
            if (Name == "PlayButton")
            {
                Console.WriteLine(onTop);
            }

            if (Input.IsMouseButtonPressed(button) && onTop)
            {
                pressed = true;
                HandleClickFocus();

                if (actionMode == ActionMode.Press)
                {
                    clickHandler?.Invoke(this);
                    onTop = false;
                    ClickSound?.Play(AudioBus);
                }
            }
        }

        if (Input.IsMouseButtonReleased(button))
        {
            if (mouseOver && pressed && onTop && actionMode == ActionMode.Release) // (mouseOver || StayPressed)
            {
                clickHandler?.Invoke(this);
                ClickSound?.Play(AudioBus);
            }

            onTop = false;
            pressed = false;
        }
    }

    private void UpdateTheme(bool isMouseOver, bool isAnyPressed)
    {
        if (StayPressed && (pressedLeft || pressedRight))
        {
            Themes.Current = Themes.Pressed;
        }
        else if (Focused)
        {
            if (isMouseOver)
            {
                Themes.Current = isAnyPressed ? Themes.Pressed : Themes.Focused;
            }
            else
            {
                Themes.Current = Focused ? Themes.Focused : Themes.Normal;
            }
        }
        else if (isMouseOver)
        {
            Themes.Current = isAnyPressed ? Themes.Pressed : Themes.Hover;
        }
        else
        {
            Themes.Current = Themes.Normal;
        }
    }

    protected override void OnThemeFileChanged(string themeFile)
    {
        Themes = FileLoader.Load<ButtonThemePack>(themeFile);
    }

    // Draw

    public override void Draw()
    {
        DrawBackground();
        DrawIcon();
        DrawText();
    }

    private void DrawBackground()
    {
        DrawRectangleThemed(
            GlobalPosition - Origin,
            ScaledSize,
            Themes.Current);
    }

    private void DrawIcon()
    {
        if (Icon is null)
        {
            return;
        }

        Vector2 iconOrigin = Icon.Size / 2;

        DrawTexture(
            Icon,
            GlobalPosition - new Vector2(Origin.X - IconMargin, 0) - iconOrigin,
            0,
            new(1),
            Color.White);
    }

    private void DrawText()
    {
        DrawText(
            displayedText,
            GetTextPosition(),
            Themes.Current.Font,
            Themes.Current.FontSize,
            Themes.Current.FontSpacing,
            Themes.Current.FontColor);
    }

    // TextDC

    //private Vector2 GetTextPosition()
    //{
    //    Vector2 textSize = Raylib_cs.Raylib.MeasureTextEx(
    //        Themes.Current.Font,
    //        TextDC,
    //        Themes.Current.FontSize,
    //        1);
    //
    //    //Vector2 textSize = Themes.Current.Font.Dimensions * new Vector2(TextDC.Length, 1);
    //
    //    float x = TextAlignment.Horizontal switch
    //    {
    //        HAlignment.Center => ScaledSize.X / 2,
    //        HAlignment.Right => ScaledSize.X - textSize.X / 2
    //    };
    //
    //    float y = TextAlignment.Vertical switch
    //    {
    //        VAlignment.Center => ScaledSize.Y / 2
    //    };
    //
    //    Vector2 origin = new(x, y);
    //
    //    Vector2 floatPosition = GlobalPosition - Origin + origin - textSize / 2 + TextOffset;
    //
    //    return new((int)floatPosition.X, (int)floatPosition.Y);
    //}

    private Vector2 GetTextPosition()
    {
        Vector2 textSize = Font.MeasureText(
            Themes.Current.Font,
            Text,
            (int)Themes.Current.FontSize,
            Themes.Current.FontSpacing);

        // Horizontal alignment calculation
        float x = TextAlignment.Horizontal switch
        {
            HorizontalAlignment.Center => ScaledSize.X / 2 - textSize.X / 2, // Center text horizontally
            HorizontalAlignment.Right => ScaledSize.X - textSize.X, // Align right
            HorizontalAlignment.Left => 0, // Align left
            _ => 0
        };

        // Vertical alignment calculation
        float y = TextAlignment.Vertical switch
        {
            VerticalAlignment.Center => ScaledSize.Y / 2 - textSize.Y / 2, // Center text vertically
            VerticalAlignment.Bottom => ScaledSize.Y - textSize.Y, // Align bottom
            VerticalAlignment.Top => 0, // Align top
            _ => 0
        };

        // Calculate the origin offset
        Vector2 origin = new(x, y);

        // Calculate final position, accounting for origin and text offset
        Vector2 floatPosition = GlobalPosition - Origin + origin + TextOffset;

        // Return the position as integers (typically for rendering in a pixel-based system)
        return new((int)floatPosition.X, (int)floatPosition.Y);
    }

    private void ResizeToFitText()
    {
        if (!AutoWidth)
        {
            return;
        }

        //int textWidth = (int)Raylib.MeasureTextEx(
        //    Themes.Current.Font,
        //    TextDC,
        //    Themes.Current.FontSize,
        //    1).X;

        int textWidth = (int)Themes.Current.Font.Dimensions.X * Text.Length;

        //Dimensions = new(textWidth + TextPadding.X * 2 + TextMargin.X, Dimensions.Y + TextMargin.Y);
        //WindowSize = new(textWidth + TextMargin.X, WindowSize.Y + TextMargin.Y);
        Size = new(textWidth + TextMargin.X, Size.Y);
    }

    private void ClipDisplayedText()
    {
        if (!ClipText)
        {
            return;
        }

        float characterWidth = GetCharacterWidth();
        int numFittingCharacters = (int)(AvailableWidth / characterWidth);

        if (numFittingCharacters <= 0)
        {
            displayedText = "";
        }
        else if (numFittingCharacters < Text.Length)
        {
            string trimmedText = Text[..numFittingCharacters];
            displayedText = GetTextClippedWithEllipsis(trimmedText);
        }
        else
        {
            displayedText = Text;
        }
    }

    private float GetCharacterWidth()
    {
        //return Raylib.MeasureTextEx(
        //    Themes.Current.Font,
        //    " ",
        //    Themes.Current.FontSize,
        //    1).X;

        return Themes.Current.Font.Dimensions.X;
    }

    private string GetTextClippedWithEllipsis(string input)
    {
        return input.Length > 3 ?
               input[..^Ellipsis.Length] + Ellipsis :
               input;
    }
}