using Cherris;

namespace Cosmocrush;

public sealed class Score : Label
{
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