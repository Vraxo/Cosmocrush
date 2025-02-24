namespace Cherris;

public class ProgressBar : Node2D
{
    public bool ShowPercentage { get; set; } = true;
    public BoxStyle BackgroundTheme { get; set; } = new();
    public BoxStyle ForegroundTheme { get; set; } = new();

    public float Percentage
    {
        get;

        set
        {
            field = Math.Clamp(value, 0, 1);
        }
    } = 0;

    // Main

    public ProgressBar()
    {
        Size = new(250, 10);
        ForegroundTheme.FillColor = DefaultTheme.Accent;
    }

    // Draw

    public override void Draw()
    {
        DrawEmpty();
        DrawFilled();
    }

    private void DrawEmpty()
    {
        DrawRectangleThemed(
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

        DrawRectangleThemed(
            GlobalPosition - Origin,
            filledSize,
            ForegroundTheme);
    }
}