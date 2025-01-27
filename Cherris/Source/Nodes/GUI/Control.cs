namespace Cherris;

public class Control : ClickableRectangle
{
    public bool Focusable { get; set; } = true;
    public bool UseArrowNavigation { get; set; } = true;
    public string? FocusNeighborTop { get; set; }
    public string? FocusNeighborBottom { get; set; }
    public string? FocusNeighborLeft { get; set; }
    public string? FocusNeighborRight { get; set; }

    public event EventHandler<bool>? FocusChanged;
    public event EventHandler? ClickedOutside;

    private bool wasFocusedLastFrame = false;

    private bool _focused = false;
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
        base.Update();

        if (UseArrowNavigation && Focused && wasFocusedLastFrame)
        {
            HandleArrowNavigation();
        }

        UpdateFocusOnOutsideClicked();
        wasFocusedLastFrame = Focused;
    }

    private void HandleArrowNavigation()
    {
        NavigateToControlIfPressed("UiLeft", FocusNeighborLeft);
        NavigateToControlIfPressed("UiUp", FocusNeighborTop);
        NavigateToControlIfPressed("UiRight", FocusNeighborRight);
        NavigateToControlIfPressed("UiDown", FocusNeighborBottom);
    }

    private void NavigateToControlIfPressed(string action, string? path)
    {
        if (Input.IsActionPressed(action) && !string.IsNullOrEmpty(path))
        {
            NavigateToControl(path);
        }
    }

    private void NavigateToControl(string controlPath)
    {
        Focused = false;
        GetNode<Control>(controlPath).Focused = true;
    }

    private void UpdateFocusOnOutsideClicked()
    {
        if (!IsMouseOver() && Input.IsMouseButtonPressed(MouseButtonCode.Left))
        {
            Focused = false;
            ClickedOutside?.Invoke(this, EventArgs.Empty);
        }
    }

    protected virtual void HandleClickFocus()
    {
        if (Focusable && IsMouseOver())
        {
            Focused = true;
        }
    }
}