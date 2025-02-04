namespace Cherris;

public class NavigationObstacle : Node2D
{
    public override void Ready()
    {
        base.Ready();

        NavigationServer.Instance.RegisterObstacle(this);

        Visible = true;
    }

    public override void Free()
    {
        base.Free();

        NavigationServer.Instance.UnregisterObstacle(this);
    }

    protected override void Draw()
    {
        base.Draw();

        DrawRectangleOutline(
            GlobalPosition - Origin,
            Size * Scale,
            Color.Pink);
    }
}