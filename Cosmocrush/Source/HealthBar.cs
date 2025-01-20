using Cherris;

namespace Cosmocrush;

public class HealthBar : ProgressBar
{
    private Player player = new();

    public override void Ready()
    {
        base.Ready();

        ProgressTheme.FillColor = Color.Green;
        player = GetNode<Player>("/root/Player");
    }

    public override void Update()
    {
        base.Update();

        Percentage = ((float)player.Health / 100);
        Position = new(Size.X / 1.5f, WindowManager.Size.Y - Size.Y * 4);
    }
}