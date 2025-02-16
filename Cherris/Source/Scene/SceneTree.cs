namespace Cherris;

public sealed class SceneTree
{
    public static SceneTree Instance { get; } = new();
    public Node? RootNode { get; set; }
    public bool Paused { get; set; }

    private readonly List<SceneTreeTimer> timers = [];

    private SceneTree() { }

    public void Process()
    {
        if (RootNode is null)
        {
            return;
        }

        ProcessNode(RootNode);
        Render();
        
        if (!Paused)
        {
            ProcessTimers();
        }
    }

    private void ProcessNode(Node node)
    {
        if (node is null || !node.Active)
        {
            return;
        }

        Node.ProcessMode effectiveMode = ComputeEffectiveMode(node);
        bool shouldProcess = ShouldProcess(effectiveMode);

        if (shouldProcess)
        {
            if (!node.IsReady)
            {
                node.Ready();
                node.IsReady = true;
            }

            node.ProcessBegin();
            node.Process();
        }

        foreach (Node child in node.Children.ToList())
        {
            ProcessNode(child);
        }

        if (shouldProcess)
        {
            node.ProcessEnd();
        }
    }

    private static Node.ProcessMode ComputeEffectiveMode(Node node)
    {
        if (node.ProcessingMode != Node.ProcessMode.Inherit)
        {
            return node.ProcessingMode;
        }

        Node? current = node.Parent;
        
        while (current is not null)
        {
            if (current.ProcessingMode != Node.ProcessMode.Inherit)
            {
                return current.ProcessingMode;
            }

            current = current.Parent;
        }

        return Node.ProcessMode.Pausable;
    }

    private bool ShouldProcess(Node.ProcessMode effectiveMode)
    {
        return effectiveMode switch
        {
            Node.ProcessMode.Disabled => false,
            Node.ProcessMode.Always => true,
            Node.ProcessMode.Pausable => !Paused,
            Node.ProcessMode.WhenPaused => Paused,
            _ => false,
        };
    }

    private void ProcessTimers()
    {
        foreach (var timer in new List<SceneTreeTimer>(timers))
        {
            timer.Process();
        }
    }

    private void Render()
    {
        if (RootNode is null)
        {
            return;
        }

        RenderNode(RootNode);
    }

    private static void RenderNode(Node node)
    {
        if (node is VisualItem { Visible: true } visualItem)
        {
            visualItem.Draw();
        }

        foreach (Node child in node?.Children.ToList() ?? [])
        {
            RenderNode(child);
        }
    }

    public SceneTreeTimer CreateTimer(float time)
    {
        timers.Add(new(time));
        return timers.Last();
    }

    public void RemoveTimer(SceneTreeTimer timer)
    {
        timers.Remove(timer);
    }

    public void ChangeScene(Node node)
    {
        RootNode?.Free();
        RootNode = node;
    }
}