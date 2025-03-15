namespace Cherris;

public class IndependantLayer : Node
{
    private readonly Dictionary<Node2D, Vector2> screenPositions = [];

    public override void ProcessBegin()
    {
        base.ProcessBegin();

        foreach (var child in Children.OfType<Node2D>())
        {
            if (!screenPositions.TryGetValue(child, out Vector2 value))
            {
                Vector2 screenPos = RenderServer.Instance.GetWorldToScreen(child.GlobalPosition);
                value = screenPos;
                screenPositions[child] = value;
            }

            child.InheritPosition = false;
            child.GlobalPosition = RenderServer.Instance.GetScreenToWorld(value);
        }
    }
}