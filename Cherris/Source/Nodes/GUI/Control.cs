using Raylib_cs;

namespace Cherris;

public class Control : ClickableRectangle
{
    public bool FocusOnClick { get; set; } = false;
    public bool ArrowNavigation { get; set; } = true;
    public string? FocusNeighborTop { get; set; }
    public string? FocusNeighborBottom { get; set; }
    public string? FocusNeighborLeft { get; set; }
    public string? FocusNeighborRight { get; set; }

    public event EventHandler<bool>? FocusChanged;
    public event EventHandler? ClickedOutside;

    private bool _focused = false;
    private bool _wasFocusedLastFrame = false; // Track focus state in previous frame
    public bool Focused
    {
        get => _focused;
        set
        {
            if (_focused != value)
            {
                _focused = value;
                FocusChanged?.Invoke(this, _focused);
            }
        }
    }

    public override void Update()
    {
        if (ArrowNavigation && Focused)
        {
            if (_wasFocusedLastFrame) // Only allow navigation if focused in previous frame
            {
                HandleArrowNavigation();
            }
        }

        UpdateFocusOnMouseOut();
        _wasFocusedLastFrame = Focused; // Process focus tracking for next frame
        base.Update();
    }

    private void HandleArrowNavigation()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Up) && !string.IsNullOrEmpty(FocusNeighborTop))
        {
            NavigateToControl(FocusNeighborTop);
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.Down) && !string.IsNullOrEmpty(FocusNeighborBottom))
        {
            NavigateToControl(FocusNeighborBottom);
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.Left) && !string.IsNullOrEmpty(FocusNeighborLeft))
        {
            NavigateToControl(FocusNeighborLeft);
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.Right) && !string.IsNullOrEmpty(FocusNeighborRight))
        {
            NavigateToControl(FocusNeighborRight);
        }
    }

    private void NavigateToControl(string controlPath)
    {
        var targetControl = GetNode<Control>(controlPath);
        Focused = false;
        targetControl.Focused = true;
    }

    private void UpdateFocusOnMouseOut()
    {
        if (!IsMouseOver() && Input.IsMouseButtonPressed(MouseButtonCode.Left))
        {
            Focused = false;
            ClickedOutside?.Invoke(this, EventArgs.Empty);
        }
    }

    protected virtual void HandleClickFocus()
    {
        if (FocusOnClick && IsMouseOver())
        {
            Focused = true;
        }
    }
}