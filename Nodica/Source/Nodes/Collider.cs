namespace Nodica;

public abstract class Collider : Node2D
{
    public Collider()
    {
        Visible = true;
    }

    public abstract bool RayIntersects(Vector2 rayStart, Vector2 rayEnd);
}