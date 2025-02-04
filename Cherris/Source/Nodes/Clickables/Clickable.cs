namespace Cherris;

public abstract class Clickable : Node2D
{
    public bool OnTopLeft = false;
    public bool OnTopRight = false;

    public override void Start()
    {
        ClickServer.Instance.Register(this);
    }

    public override void Free()
    {
        ClickServer.Instance.Unregister(this);
        base.Free();
    }

    public abstract bool IsMouseOver();
}