﻿namespace Cherris;

public class Control : ClickableRectangle
{
    public bool Focusable { get; set; } = true;
    public bool Navigable { get; set; } = true;
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

    public string AudioBus { get; set; } = "Master";
    public Sound? FocusGainedSound { get; set; }

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
            FocusChanged?.Invoke(this);

            if (_focused)
            {
                FocusGained?.Invoke(this);

                if (FocusGainedSound is not null)
                {
                    FocusGainedSound?.Play(AudioBus);
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

        if (Navigable && Focused && wasFocusedLastFrame)
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
        NavigateToControlIfPressed("UiNext", FocusNeighborBottom);
        NavigateToControlIfPressed("UiPrevious", FocusNeighborBottom);
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