namespace Cherris;

public abstract class Slider : Control
{
    public float Value { get; set; } = 0.5f;
    public float MinValue { get; set; } = 0;
    public float MaxValue { get; set; } = 1f;
    private float step = 0.1f;
    public float Step
    {
        get { return step; }
        set { step = Math.Max(value, 0); }
    }
    public BoxTheme BackgroundTheme { get; set; } = new BoxTheme();
    public BoxTheme ForegroundTheme { get; set; } = new BoxTheme();
    public ButtonThemePack GrabberTheme { get; set; } = new ButtonThemePack();
    public Vector2 GrabberSize { get; set; } = new Vector2(20, 20);

    protected bool grabberPressed;
    protected bool grabberHovered;
    protected bool trackHovered;
    protected Vector2 trackPosition;
    protected float trackMin;
    protected float trackMax;

    public Slider()
    {
        Size = new Vector2(512, 16);
        Focusable = true;
        UseArrowNavigation = true;
        ForegroundTheme.FillColor = DefaultTheme.Accent;
        ForegroundTheme.Roundness = 1;
        BackgroundTheme.Roundness = 1;
        GrabberTheme.Roundness = 1;
    }

    public override void Update()
    {
        base.Update();

        if (Input.IsMouseButtonPressed(MouseButtonCode.Left))
        {
            HandleClickFocus();
        }

        if (Focused)
        {
            HandleKeyboardNavigation();
            HandleFocusExit();
        }

        CalculateTrackBounds();
        UpdateHoverStates();
        HandleInput();
        UpdateGrabberTheme();
    }

    protected virtual void HandleKeyboardNavigation()
    {
        // To be implemented in derived classes
    }

    protected virtual void HandleFocusExit()
    {
        if (Input.IsActionPressed("UiAccept") || (Input.IsMouseButtonPressed(MouseButtonCode.Left) && !IsMouseOver()))
        {
            Focused = false;
        }
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

    protected override void Draw()
    {
        base.Draw();
        DrawBackground();
        DrawForeground();
        DrawGrabber();
    }

    private void DrawBackground()
    {
        DrawRectangleThemed(trackPosition, Size, BackgroundTheme);
    }

    private void DrawGrabber()
    {
        Vector2 grabberPos = CalculateGrabberPosition();
        var themeState = Focused ? GrabberTheme.Focused :
            grabberPressed ? GrabberTheme.Pressed :
            grabberHovered ? GrabberTheme.Hover :
            GrabberTheme.Normal;

        DrawRectangleThemed(grabberPos, GrabberSize, themeState);
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