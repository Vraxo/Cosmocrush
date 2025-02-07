namespace Cherris;

public class Control : ClickableRectangle
{
    public bool Focusable { get; set; } = true;
    public bool Navigable { get; set; } = true;
    public bool RapidNavigation { get; set; } = true;
    public string? FocusNeighborTop { get; set; }
    public string? FocusNeighborBottom { get; set; }
    public string? FocusNeighborLeft { get; set; }
    public string? FocusNeighborRight { get; set; }
    public string? FocusNeighborNext { get; set; }
    public string? FocusNeighborPrevious { get; set; }

    public delegate void ControlEventHandler(Control control);
    public event ControlEventHandler? FocusChanged;
    public event ControlEventHandler? FocusGained;
    public event ControlEventHandler? WasDisabled;
    public event ControlEventHandler? ClickedOutside;

    private bool wasFocusedLastFrame = false;
    private Dictionary<string, float> actionHoldTimes = new Dictionary<string, float>();
    private const float InitialDelay = 0.5f;
    private const float RepeatInterval = 0.1f;

    public string AudioBus { get; set; } = "Master";
    public Sound? FocusGainedSound { get; set; }

    public bool Disabled
    {
        get;
        set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            WasDisabled?.Invoke(this);
        }
    } = false;

    public bool Focused
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }
            field = value;
            FocusChanged?.Invoke(this);

            if (field)
            {
                FocusGained?.Invoke(this);

                if (FocusGainedSound is not null)
                {
                    FocusGainedSound?.Play(AudioBus);
                }
            }
        }
    } = false;

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

        if (Navigable && Focused && wasFocusedLastFrame)
        {
            HandleArrowNavigation();
        }

        UpdateFocusOnOutsideClicked();
        wasFocusedLastFrame = Focused;
    }

    private void HandleArrowNavigation()
    {
        var actions = new (string Action, string? Path)[]
        {
            ("UiLeft", FocusNeighborLeft),
            ("UiUp", FocusNeighborTop),
            ("UiRight", FocusNeighborRight),
            ("UiDown", FocusNeighborBottom),
            ("UiNext", FocusNeighborNext),
            ("UiPrevious", FocusNeighborPrevious)
        };

        foreach (var entry in actions)
        {
            if (string.IsNullOrEmpty(entry.Path)) continue;

            if (RapidNavigation)
            {
                if (Input.IsActionDown(entry.Action))
                {
                    if (!actionHoldTimes.ContainsKey(entry.Action))
                    {
                        actionHoldTimes[entry.Action] = 0f;
                    }

                    actionHoldTimes[entry.Action] += TimeServer.Delta;
                    float holdTime = actionHoldTimes[entry.Action];

                    bool shouldNavigate = (holdTime <= TimeServer.Delta + float.Epsilon) ||
                        (holdTime >= InitialDelay && (holdTime - InitialDelay) % RepeatInterval < TimeServer.Delta);

                    if (shouldNavigate)
                    {
                        NavigateToControl(entry.Path, entry.Action, holdTime);
                    }
                }
                else
                {
                    actionHoldTimes[entry.Action] = 0f;
                }
            }
            else
            {
                if (Input.IsActionPressed(entry.Action))
                {
                    NavigateToControl(entry.Path, entry.Action, 0f);
                }
            }
        }
    }

    private void NavigateToControl(string controlPath, string action, float holdTime)
    {
        var neighbor = GetNodeOrNull<Control>(controlPath);

        if (neighbor is null)
        {
            Log.Error($"[Control] [{Name}] NavigateToControl: Could not find '{controlPath}'.");
            return;
        }

        if (neighbor.Disabled)
        {
            return;
        }

        if (RapidNavigation)
        {
            neighbor.actionHoldTimes[action] = holdTime;
        }

        neighbor.Focused = true;
        Focused = false;
    }

    private void UpdateFocusOnOutsideClicked()
    {
        if (!IsMouseOver() && Input.IsMouseButtonPressed(MouseButtonCode.Left))
        {
            Focused = false;
            ClickedOutside?.Invoke(this);
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