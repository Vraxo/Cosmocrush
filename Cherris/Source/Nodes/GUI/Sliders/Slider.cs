﻿namespace Cherris;

public abstract class Slider : Control
{
    public float Value { get; set; } = 0.5f;
    public float MinValue { get; set; } = 0;
    public float MaxValue { get; set; } = 1f;
    public Sound? MoveSound { get; set; }
    public SliderStyle Theme { get; set; } = new();
    public ButtonStylePack GrabberTheme { get; set; } = new();
    public Vector2 GrabberSize { get; set; } = new(20);

    protected bool grabberPressed;
    protected bool grabberHovered;
    protected bool trackHovered;
    protected Vector2 trackPosition;
    protected float trackMin;
    protected float trackMax;

    public float Step
    {
        get;

        set 
        {
            field = Math.Max(value, 0); 
        }
    } = 0.01f;

    public Slider()
    {
        Size = new(512, 16);
        Focusable = true;
        Navigable = true;
        Theme.Foreground.FillColor = DefaultTheme.Accent;
        Theme.Foreground.Roundness = 1;
        Theme.Foreground.Roundness = 1;
        Theme.Grabber.Roundness = 1;
    }

    public override void Process()
    {
        base.Process();

        if (Input.IsMouseButtonPressed(MouseButtonCode.Left))
        {
            HandleClickFocus();
        }

        if (Focused)
        {
            HandleKeyboardNavigation();
            OnFocusLost();
        }

        CalculateTrackBounds();
        UpdateHoverStates();
        HandleInput();
        UpdateGrabberTheme();
    }

    protected abstract void HandleKeyboardNavigation();

    protected virtual void OnFocusLost()
    {
        if (Input.IsActionPressed("UiAccept") || (Input.IsMouseButtonPressed(MouseButtonCode.Left) && !IsMouseOver()))
        {
            Focused = false;
        }
    }

    protected override void OnThemeFileChanged(string themeFile)
    {
        Theme = FileLoader.Load<SliderStyle>(themeFile);
    }

    protected void PlaySound()
    {
        MoveSound?.Play(AudioBus);
    }

    protected abstract void CalculateTrackBounds();
    protected abstract void UpdateHoverStates();
    protected abstract void HandleInput();
    protected abstract void UpdateGrabberTheme();
    protected abstract float ConvertPositionToValue(float position);
    protected abstract void DrawForeground();
    protected abstract Vector2 CalculateGrabberPosition();

    protected float ApplyStep(float value)
    {
        if (Step <= 0)
        {
            return Math.Clamp(value, MinValue, MaxValue);
        }

        float steppedValue = MinValue + (float)Math.Round((value - MinValue) / Step) * Step;
        return Math.Clamp(steppedValue, MinValue, MaxValue);
    }

    public override void Draw()
    {
        base.Draw();
        DrawBackground();
        DrawForeground();
        DrawGrabber();
    }

    private void DrawBackground()
    {
        DrawRectangleThemed(
            trackPosition,
            Size,
            Theme.Background);
    }

    private void DrawGrabber()
    {
        Vector2 grabberPos = CalculateGrabberPosition();

        BoxStyle themeState = Focused ? Theme.Grabber.Focused :
            grabberPressed ? Theme.Grabber.Pressed :
            grabberHovered ? Theme.Grabber.Hover :
            Theme.Grabber.Hover;

        DrawRectangleThemed(
            grabberPos,
            GrabberSize,
            themeState);
    }

    protected override void HandleClickFocus()
    {
        if (Focusable && (trackHovered || grabberHovered))
        {
            Focused = true;
        }
        else if (IsMouseOver())
        {
            // Maintain focus if clicking within control
            Focused = true;
        }
    }
}