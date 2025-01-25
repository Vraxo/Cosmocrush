using Cherris;

namespace Cosmocrush;

public class MainCamera : Camera
{
    public override void Ready()
    {
        base.Ready();

        Zoom = 1;
        SetAsActive();
        InheritPosition = false;
    }

    public override void Update()
    {
        base.Update();

        GlobalPosition = GetNode<Player>("/root/Player").GlobalPosition;
    }

    protected override void Draw()
    {
        base.Draw();

        DrawRectangle(GlobalPosition, new(100, 100), Color.Red);
    }
}