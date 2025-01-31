using static Cherris.Button;

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

    public delegate void ControlEventHandler(Control control);
    public event ControlEventHandler? FocusGained;
    public event ControlEventHandler? WasDisabled;

    private bool wasFocusedLastFrame = false;

    public string AudioBus { get; set; } = "Master";
    public Audio? FocusGainedAudio { get; set; }

    private bool _disabled = false;
    public bool Disabled
    {
        get => _disabled;
        set
        {
            if (value == _disabled)
            {
                return;
            }

            _disabled = value;
            WasDisabled?.Invoke(this);
        }
    }

    private bool _focused = false;
    public bool Focused
    {
        get => _focused;
        set
        {
            if (_focused == value)
            {
                return;
            }
            _focused = value;
            FocusChanged?.Invoke(this, _focused);

            if (_focused)
            {
                FocusGained?.Invoke(this);

                if (FocusGainedAudio is not null)
                {
                    AudioManager.PlaySound(FocusGainedAudio, AudioBus);
                }
            }
            else
            {

            }
        }
    }

    public string ThemeFile
    {
        set
        {
            OnThemeFileChanged(value);
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
        var neighbor = GetNode<Control>(controlPath);

        if (neighbor.Disabled)
        {
            return;
        }

        neighbor.Focused = true;
        Focused = false;
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

    protected virtual void OnThemeFileChanged(string themeFile) { }
}