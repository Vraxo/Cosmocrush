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
                child.Position = RenderManager.Instance.GetScreenToWorld(child.GlobalPosition);
            }
        }
    }
}