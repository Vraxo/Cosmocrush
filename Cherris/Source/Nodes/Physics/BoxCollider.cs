namespace Cherris;

public class BoxCollider : Node2D
{
    public List<int> CollisionLayers { get; set; } = [0];
    public bool Enabled { get; set; } = true;
    public float Density { get; set; } = 1;
    public float Friction { get; set; } = 0.5f;
    public float Restitution { get; set; } = 0;
    public bool IsSensor { get; set; } = false;

    public BoxCollider()
    {
        Size = new(32, 32);
    }
}