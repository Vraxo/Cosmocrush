namespace Cherris;

public class IndependantLayer : Node
{
    private readonly Dictionary<Node2D, Vector2> nodePositions = [];

    public override void ProcessBegin()
    {
        base.ProcessBegin();

        foreach (var child in Children.OfType<Node2D>())
        {
            nodePositions.Add(child, child.GlobalPosition);

            child.InheritPosition = false;
            child.GlobalPosition = RenderServer.Instance.GetScreenToWorld(child.GlobalPosition);
        }
    }

    public override void ProcessEnd()
    {
        base.ProcessEnd();

        foreach (Node2D child in nodePositions.Keys)
        {
            child.GlobalPosition = nodePositions[child];
        }

        nodePositions.Clear();
    }
}