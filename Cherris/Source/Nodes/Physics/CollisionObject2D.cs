namespace Cherris;

public class CollisionObject2D : Node2D
{
    public ushort CollisionLayer { get; set; } = 1;
    public ushort CollisionMask { get; set; } = 1;
    public BoxCollider Collider { get; set; }

    public override void Ready()
    {
        base.Ready();
        // Find the first BoxCollider child
        foreach (var child in Children)
        {
            if (child is not BoxCollider collider)
            {
                continue;
            }
            Collider = collider;
            break;
        }
    }
}