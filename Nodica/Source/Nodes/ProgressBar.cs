namespace Nodica;

public class ProgressBar : Node2D
{
    public BoxTheme BackgroundTheme = new();

    public BoxTheme ProgressTheme = new()
    { 
        FillColor = DefaultTheme.Accent
    };

    private float _percentage = 0;
    public float Percentage
    {
        get => _percentage;

        set
        {
            _percentage = Math.Clamp(value, 0, 1);
        }
    }

    public ProgressBar()
    {
        Size = new(250, 10);
    }

    protected override void Draw()
    {
        DrawEmpty();
        DrawFilled();
    }

    private void DrawEmpty()
    {
        DrawThemedRectangle(
            GlobalPosition - Origin,
            Size,
            BackgroundTheme);
    }

    private void DrawFilled()
    {
        if (Percentage == 0)
        {
            return;
        }

        Vector2 filledSize = new(Size.X * Percentage, Size.Y);

        DrawThemedRectangle(
            GlobalPosition - Origin,
            filledSize,
            ProgressTheme);
    }
}