namespace Cherris;

public class HSlider : Slider
{
    public HSliderDirection Direction { get; set; } = HSliderDirection.LeftToRight;

    protected override void CalculateTrackBounds()
    {
        trackPosition = GlobalPosition - Origin;
        trackMin = trackPosition.X;
        trackMax = trackPosition.X + Size.X;
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
                float clampedX = Math.Clamp(Input.MousePosition.X, trackMin, trackMax);
                Value = ConvertPositionToValue(clampedX);
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

        float clampedX = Math.Clamp(Input.MousePosition.X, trackMin, trackMax);
        Value = ConvertPositionToValue(clampedX);
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
        if (Direction == HSliderDirection.RightToLeft)
        {
            normalized = 1 - normalized;
        }

        float rawValue = normalized * (MaxValue - MinValue) + MinValue;
        return ApplyStep(rawValue);
    }

    protected override void DrawForeground()
    {
        float width = Size.X * ((Value - MinValue) / (MaxValue - MinValue));
        if (Direction == HSliderDirection.RightToLeft)
        {
            DrawRectangleThemed(
                new Vector2(trackPosition.X + Size.X - width, trackPosition.Y),
                new Vector2(width, Size.Y),
                ForegroundTheme
            );
        }
        else
        {
            DrawRectangleThemed(trackPosition, new Vector2(width, Size.Y), ForegroundTheme);
        }
    }

    protected override Vector2 CalculateGrabberPosition()
    {
        float normalizedValue = (Value - MinValue) / (MaxValue - MinValue);
        if (Direction == HSliderDirection.RightToLeft)
        {
            normalizedValue = 1 - normalizedValue;
        }
        float centerX = trackMin + normalizedValue * Size.X;
        float yPos = trackPosition.Y + (Size.Y / 2) - GrabberSize.Y / 2;
        return new Vector2(centerX - GrabberSize.X / 2, yPos);
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
        base.HandleKeyboardNavigation();

        if (Focused)
        {
            if (Input.IsActionPressed("UiLeft"))
            {
                Value = ApplyStep(Value - Step);
            }
            else if (Input.IsActionPressed("UiRight"))
            {
                Value = ApplyStep(Value + Step);
            }
        }
    }
}