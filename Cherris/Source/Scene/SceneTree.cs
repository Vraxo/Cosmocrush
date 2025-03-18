using System.Runtime.CompilerServices;

namespace Cherris;

public sealed class SceneTree
{
    public static SceneTree Instance { get; } = new();

    public Node? RootNode { get; set; }
    public bool Paused { get; set; }

    private readonly ConditionalWeakTable<Node, object> readyNodes = [];
    private readonly List<SceneTreeTimer> timers = [];
    private readonly List<Tween> activeTweens = [];

    private SceneTree() { }

    // Main processing loop

    public void Process()
    {
        if (RootNode is null)
        {
            return;
        }

        ProcessNode(RootNode);
        Render();

        if (Input.IsKeyPressed(KeyCode.Enter))
        {
            RootNode.PrintChildren();
        }

        ProcessTweens();

        if (!Paused)
        {
            ProcessTimers();
        }
    }

    // Node processing and traversal

    private void ProcessNode(Node node)
    {
        if (node is null || !node.Active)
        {
            return;
        }

        Node.ProcessMode effectiveMode = ComputeEffectiveProcessMode(node);
        bool shouldProcess = ShouldProcess(effectiveMode);

        if (shouldProcess)
        {
            EnsureNodeReady(node);
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

    private void EnsureNodeReady(Node node)
    {
        if (!readyNodes.TryGetValue(node, out _))
        {
            node.Ready();
            readyNodes.Add(node, null);
        }
    }

    private static Node.ProcessMode ComputeEffectiveProcessMode(Node node)
    {
        if (node.ProcessingMode != Node.ProcessMode.Inherit)
        {
            return node.ProcessingMode;
        }

        Node? current = node.Parent;

        while (current != null)
        {
            if (current.ProcessingMode != Node.ProcessMode.Inherit)
            {
                return current.ProcessingMode;
            }

            current = current.Parent;
        }

        return Node.ProcessMode.Pausable;
    }

    private bool ShouldProcess(Node.ProcessMode mode) => mode switch
    {
        Node.ProcessMode.Disabled => false,
        Node.ProcessMode.Always => true,
        Node.ProcessMode.Pausable => !Paused,
        Node.ProcessMode.WhenPaused => Paused,
        _ => false
    };

    // Rendering

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

        foreach (Node child in node.Children.ToList())
        {
            RenderNode(child);
        }
    }

    // Timers

    public SceneTreeTimer CreateTimer(float time)
    {
        SceneTreeTimer timer = new(time);
        timers.Add(timer);
        return timer;
    }

    public void RemoveTimer(SceneTreeTimer timer)
    {
        timers.Remove(timer);
    }

    private void ProcessTimers()
    {
        foreach (SceneTreeTimer timer in timers.ToList())
        {
            timer.Process();
        }
    }

    // Scene

    public void ChangeScene(Node node)
    {
        RootNode?.Free();
        RootNode = node;
    }

    // Tweens

    public Tween CreateTween(Node creatorNode, Node.ProcessMode processMode = Node.ProcessMode.Inherit)
    {
        Tween tween = new(creatorNode, processMode);
        activeTweens.Add(tween);
        return tween;
    }

    private void ProcessTweens()
    {
        foreach (Tween tween in activeTweens.ToList())
        {
            if (tween.ShouldProcess(Paused))
            {
                tween.Update(Time.Delta);
            }

            if (!tween.Active)
            {
                activeTweens.Remove(tween);
            }
        }
    }
}