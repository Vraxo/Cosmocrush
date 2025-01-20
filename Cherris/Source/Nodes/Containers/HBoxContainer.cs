namespace Cherris;

public class HBoxContainer : Node2D
{
    public float Spacing { get; set; } = 16;

    public override void Update()
    {
        base.Update();

        float width = 0;

        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i] is Node2D child)
            {
                child.Position = new(width, 0);
                width += child.Size.X + Spacing;
            }
        }
    }
}