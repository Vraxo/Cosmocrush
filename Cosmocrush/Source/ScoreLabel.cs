using Cherris;

namespace Cosmocrush;

public sealed class ScoreLabel : Label
{
    public int Points
    {
        get;
        set
        {
            Text = $"Score: {Points + 1}";
            field = value;
        }
    } = 0;

    public override void Ready()
    {
        base.Ready();

        InheritPosition = false;
    }

    public override void Process()
    {
        base.Process();

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float screenX = Size.X;
        float screenY = Size.Y * 4;
        Vector2 screenPos = new(screenX, screenY);

        Position = RenderServer.Instance.GetScreenToWorld(screenPos);
    }
}