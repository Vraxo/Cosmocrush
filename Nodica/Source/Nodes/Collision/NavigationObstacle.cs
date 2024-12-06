namespace Nodica;

public class NavigationObstacle : Node2D
{
    public override void Ready()
    {
        base.Ready();

        NavigationManager.Instance.RegisterObstacle(this);
    }

    public override void Destroy()
    {
        base.Destroy();

        NavigationManager.Instance.UnregisterObstacle(this);
    }

    protected override void Draw()
    {
        base.Draw();

        DrawRectangleOutline(
            GlobalPosition - Origin,
            Size,
            Color.SkyBlue);
    }
}