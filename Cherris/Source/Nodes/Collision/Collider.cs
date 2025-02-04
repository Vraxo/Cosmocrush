namespace Cherris;

public abstract class Collider : Node2D
{
    public bool IsStatic { get; set; } = false;

    public bool Enabled { get; set; } = true;
    public List<int> CollisionLayers { get; set; } = [0];
    public Color Color { get; set; } = new(0, 0, 128, 128);

    public Collider()
    {
        Visible = true;
        ActiveChanged += OnActiveChanged;
    }

    public override void Ready()
    {
        Register();
    }

    private void OnActiveChanged(Node sender, bool active)
    {
        if (active)
        {
            Register();
        }
        else
        {
            Unregister();
        }
    }

    public override void Free()
    {
        base.Free();

        Unregister();
    }

    public abstract bool RayIntersects(Vector2 rayStart, Vector2 rayEnd);

    protected abstract void Register();
    protected abstract void Unregister();
}