namespace Cherris;

public class VSlider : Slider
{
    public VSliderDirection Direction { get; set; } = VSliderDirection.TopToBottom;

    protected override void CalculateTrackBounds()
    {
        trackPosition = GlobalPosition - Origin;
        trackMin = trackPosition.Y;
        trackMax = trackPosition.Y + Size.Y;
    }

    protected override void UpdateHoverStates()
    {
        Vector2 mousePos = Input.MousePosition;

        trackHovered = mousePos.X >= trackPosition.X &&
                      mousePos.X <= trackPosition.X + Size.X &&
                      mousePos.Y >= trackPosition.Y &&
                      mousePos.Y <= trackPosition.Y + Size.Y;

        Vector2 grabberPos = CalculateGrabberPosition();
        grabberHovered = mousePos.X >= grabberPos.X &&
                        mousePos.X <= grabberPos.X + GrabberSize.X &&
                        mousePos.Y >= grabberPos.Y &&
                        mousePos.Y <= grabberPos.Y + GrabberSize.Y;
    }

    protected override void HandleInput()
    {
        HandleMousePress();
        HandleMouseDrag();
        HandleMouseWheel();
    }

    private void HandleMousePress()
    {
        if (Input.IsMouseButtonPressed(MouseButtonCode.Left))
        {
            if (trackHovered && !grabberHovered)
            {
                float clampedY = Math.Clamp(Input.MousePosition.Y, trackMin, trackMax);
                Value = ConvertPositionToValue(clampedY);
                grabberPressed = true;
            }
            else
            {
                grabberPressed = grabberHovered;
            }
        }
        else if (Input.IsMouseButtonReleased(MouseButtonCode.Left))
        {
            grabberPressed = false;
        }
    }

    private void HandleMouseDrag()
    {
        if (!grabberPressed) return;

        float clampedY = Math.Clamp(Input.MousePosition.Y, trackMin, trackMax);
        Value = ConvertPositionToValue(clampedY);
    }

    private void HandleMouseWheel()
    {
        if (!trackHovered && !grabberHovered) return;

        float wheelDelta = Input.GetMouseWheelMovement();
        if (wheelDelta == 0) return;

        Value = ApplyStep(Value + (wheelDelta * Step));
    }

    protected override float ConvertPositionToValue(float position)
    {
        float normalized = (position - trackMin) / (trackMax - trackMin);
        if (Direction == VSliderDirection.BottomToTop)
        {
            normalized = 1 - normalized;
        }

        float rawValue = normalized * (MaxValue - MinValue) + MinValue;
        return ApplyStep(rawValue);
    }

    protected override void DrawForeground()
    {
        float height = Size.Y * ((Value - MinValue) / (MaxValue - MinValue));
        if (Direction == VSliderDirection.BottomToTop)
        {
            DrawRectangleThemed(
                new(trackPosition.X, trackPosition.Y + Size.Y - height),
                new(Size.X, height),
                Theme.Foreground
            );
        }
        else
        {
            DrawRectangleThemed(
                trackPosition,
                new(Size.X, height),
                Theme.Foreground);
        }
    }

    protected override Vector2 CalculateGrabberPosition()
    {
        float normalizedValue = (Value - MinValue) / (MaxValue - MinValue);
        if (Direction == VSliderDirection.BottomToTop)
        {
            normalizedValue = 1 - normalizedValue;
        }
        float centerY = trackMin + normalizedValue * Size.Y;
        float xPos = trackPosition.X + (Size.X / 2) - GrabberSize.X / 2;
        return new Vector2(xPos, centerY - GrabberSize.Y / 2);
    }

    protected override void UpdateGrabberTheme()
    {
        if (grabberPressed)
        {
            GrabberTheme.Current = GrabberTheme.Pressed;
        }
        else if (grabberHovered)
        {
            GrabberTheme.Current = GrabberTheme.Hover;
        }
        else
        {
            GrabberTheme.Current = GrabberTheme.Normal;
        }
    }

    protected override void HandleKeyboardNavigation()
    {
        if (Focused)
        {
            if (Input.IsActionPressed("UiUp"))
            {
                Value = ApplyStep(Value + Step);
            }
            else if (Input.IsActionPressed("UiDown"))
            {
                Value = ApplyStep(Value - Step);
            }
        }
    }
}