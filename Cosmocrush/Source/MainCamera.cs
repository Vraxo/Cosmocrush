using Cherris;

namespace Cosmocrush;

public class MainCamera : Camera
{
    private Player? player;

    public override void Ready()
    {
        base.Ready();

        Zoom = 1;
        InheritPosition = false;

        player = GetNode<Player>("/root/Player");

        SetAsActive();
    }

    public override void Process()
    {
        base.Process();

        if (player is not null)
        {
            GlobalPosition = player.GlobalPosition;
        }
    }
}