using Cherris;

namespace Cosmocrush;

public class HealthBar : ProgressBar
{
    private Player player = new();

    public override void Ready()
    {
        base.Ready();

        InheritPosition = false;
        ProgressTheme.FillColor = Color.Green;
        player = GetNode<Player>("/root/Player");
    }

    public override void Update()
    {
        base.Update();

        UpdatePosition();
        UpdatePercentage();
    }

    private void UpdatePercentage()
    {
        Percentage = ((float)player.Health / 100);
    }

    private void UpdatePosition()
    {
        float x = Size.X / 1.5f;
        float y = VisualServer.WindowSize.Y - Size.Y * 4;

        GlobalPosition = new(x, y);
        //GlobalPosition = Raylib_cs.Raylib.GetScreenToWorld2D(GlobalPosition, RenderServer.Instance.Camera);
    }
}