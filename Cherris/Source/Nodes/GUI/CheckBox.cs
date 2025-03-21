﻿using Raylib_cs;

namespace Cherris;

public class CheckBox : Control
{
    public enum ClickMode { Limited, Limitless }
    public enum ActionMode { Release, Press }
    public enum ClickBehavior { Left, Right, Both }

    #region [ - - - Properties & Fields - - - ]

    public ButtonStylePack BackgroundStyles { get; set; } = new();
    public BoxStyle CheckStyles { get; set; } = new();
    public Vector2 CheckSize { get; set; } = new();
    public ActionMode LeftClickActionMode { get; set; } = ActionMode.Release;
    public ActionMode RightClickActionMode { get; set; } = ActionMode.Release;
    public bool StayPressed { get; set; } = false;
    public ClickBehavior Behavior { get; set; } = ClickBehavior.Both;
    public bool Checked { get; private set; } = false;

    public bool PressedLeft = false;
    public bool PressedRight = false;

    public event EventHandler<bool>? Toggled;

    #endregion

    public CheckBox()
    {
        Size = new(20, 20);
        FocusChanged += OnFocusChanged;
        CheckSize = Size / 2;

        BackgroundStyles.Roundness = 1f;
        CheckStyles.Roundness = 1f;
        CheckStyles.FillColor = DefaultTheme.Accent;
    }

    public override void Process()
    {
        if (!Disabled)
        {
            HandleClicks();
            HandleKeyboardInput();
        }

        Draw();
        base.Process();
    }

    public void Toggle()
    {
        Checked = !Checked;
        Toggled?.Invoke(this, Checked);
    }

    private void OnFocusChanged(Control control)
    {
        BackgroundStyles.Current = 
            control.Focused ?
            BackgroundStyles.Focused :
            BackgroundStyles.Normal;
    }

    private void HandleKeyboardInput()
    {
        if (Focused && Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            Toggle();
        }
    }

    private void HandleClicks()
    {
        if (Disabled) return;

        bool mouseOver = IsMouseOver();
        bool anyPressed = false;

        if (Behavior == ClickBehavior.Left || Behavior == ClickBehavior.Both)
        {
            HandleClick(
                ref PressedLeft,
                MouseButtonCode.Left,
                LeftClickActionMode);

            if (PressedLeft) anyPressed = true;
        }

        if (Behavior == ClickBehavior.Right || Behavior == ClickBehavior.Both)
        {
            HandleClick(
                ref PressedRight,
                MouseButtonCode.Right,
                RightClickActionMode);

            if (PressedRight) anyPressed = true;
        }

        if (StayPressed && (PressedLeft || PressedRight))
        {
            BackgroundStyles.Current = BackgroundStyles.Pressed;
        }
        else if (Focused)
        {
            if (mouseOver)
            {
                BackgroundStyles.Current = anyPressed ? BackgroundStyles.Pressed : BackgroundStyles.Focused;
            }
            else
            {
                BackgroundStyles.Current = Focused ? BackgroundStyles.Focused : BackgroundStyles.Normal;
            }
        }
        else if (mouseOver)
        {
            BackgroundStyles.Current = anyPressed ? BackgroundStyles.Pressed : BackgroundStyles.Hover;
        }
        else
        {
            BackgroundStyles.Current = BackgroundStyles.Normal;
        }
    }

    private void HandleClick(ref bool pressed, MouseButtonCode button, ActionMode actionMode)
    {
        if (Disabled) return;

        bool mouseOver = IsMouseOver();

        if (mouseOver)
        {
            if (Input.IsMouseButtonPressed(button))
            {
                pressed = true;
                HandleClickFocus();

                if (actionMode == ActionMode.Press)
                {
                    Toggle();
                }
            }
        }

        if (Input.IsMouseButtonReleased(button))
        {
            if (mouseOver && pressed && actionMode == ActionMode.Release)
            {
                Toggle();
            }

            pressed = false;
        }
    }

    public override void Draw()
    {
        DrawRectangleThemed(
            GlobalPosition - Origin,
            Size,
            BackgroundStyles.Current);

        if (!Checked)
        {
            return;
        }

        DrawRectangleThemed(
            GlobalPosition - Origin / 2,
            CheckSize,
            CheckStyles);
    }
}