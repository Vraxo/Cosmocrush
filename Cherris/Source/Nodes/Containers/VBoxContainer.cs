namespace Cherris;

public class VBoxContainer : Node2D
{
    public float Spacing { get; set; } = 16;

    public override void Update()
    {
        base.Update();

        float height = 0;

        foreach (var child in Children.OfType<Node2D>())
        {
            child.Position = new(0, height);
            height += child.Size.Y + Spacing;
        }
    }
}