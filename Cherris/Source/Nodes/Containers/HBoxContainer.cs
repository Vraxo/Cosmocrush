namespace Cherris;

public class HBoxContainer : Node2D
{
    public float Spacing { get; set; } = 16;

    public override void Process()
    {
        base.Process();

        float width = 0;

        foreach (var child in Children.OfType<Node2D>())
        {
            child.Position = new(width, 0);
            width += child.Size.X + Spacing;
        }
    }
}