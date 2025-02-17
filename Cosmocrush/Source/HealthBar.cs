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
        float x = Size.X / 1.5f;
        float y = VisualServer.WindowSize.Y - Size.Y * 4;

        //Position = new(x, y);

        Layer = 100;

        //DrawLine(
        //    GlobalPosition,
        //    player!.GlobalPosition,
        //    10,
        //    Color.Red);

        //Position = Raylib_cs.Raylib.GetScreenToWorld2D(GlobalPosition, RenderServer.Instance.Camera);
    }
}