using Cherris;

namespace Cosmocrush;

public class HealthBar : ProgressBar
{
    private Player? player;

    public override void Ready()
    {
        base.Ready();

        player = GetNode<Player>("/root/Player");
    }

    public override void Process()
    {
        base.Process();

        UpdatePosition();
        UpdatePercentage();
    }

    private void UpdatePercentage()
    {
        Percentage = (float)player!.Health / 100;
    }

    private void UpdatePosition()
    {
        //float screenX = Size.X / 5;
        //float screenY = DisplayServer.WindowSize.Y - Size.Y * 4;
        //Vector2 screenPos = new(screenX, screenY);
        //
        //Position = RenderServer.Instance.GetScreenToWorld(screenPos);
    }
}