namespace Cherris;

public class NavigationObstacle : Node2D
{
    public NavigationObstacle()
    {
        Visible = false;
    }

    public override void Ready()
    {
        base.Ready();

        NavigationServer.Instance.RegisterObstacle(this);
    }

    public override void Free()
    {
        base.Free();

        NavigationServer.Instance.UnregisterObstacle(this);
    }

    public override void Draw()
    {
        base.Draw();

        DrawRectangleOutline(
            GlobalPosition - Origin,
            Size * Scale,
            Color.Pink);
    }
}