namespace Cherris;

public class IndependantLayer : Node
{
    public override void ProcessEnd()
    {
        base.ProcessEnd();

        foreach (Node node in Children)
        {
            if (node is Node2D child)
            {
                child.Position = RenderServer.Instance.GetScreenToWorld(child.GlobalPosition);
            }
        }
    }
}