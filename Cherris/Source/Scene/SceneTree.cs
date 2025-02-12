namespace Cherris;

public sealed class SceneTree
{
    public static SceneTree Instance { get; } = new();

    public Node? RootNode { get; set; }
    public bool Paused { get; private set; } = false;

    private readonly List<SceneTreeTimer> timers = [];

    // Main

    private SceneTree() { }

    public void Process()
    {
        if (Paused)
        {
            return;
        }

        RootNode?.Process();
        Render();
        ProcessTimers();

        if (Input.IsKeyPressed(KeyCode.Enter))
        {
            PrintTree();
        }
    }

    public void ProcessTimers()
    {
        foreach (var timer in new List<SceneTreeTimer>(timers))
        {
            timer.Process();
        }
    }

    public void ChangeScene(Node node)
    {
        RootNode?.Free();
        RootNode = node;

        //node.Name = node.GetType().Name;
    }

    public void PrintTree()
    {
        RootNode?.PrintChildren();
    }

    // Render

    public void Render()
    {
        if (RootNode is not null)
        {
            RenderVisualItems(RootNode);
        }
    }

    private static void RenderVisualItems(Node node)
    {
        if (node is VisualItem visualItem && visualItem.Visible)
        {
            visualItem.Draw();
        }

        foreach (Node child in node.Children)
        {
            RenderVisualItems(child);
        }
    }

    // Timer

    public SceneTreeTimer CreateTimer(float time)
    {
        timers.Add(new(time));
        return timers.Last();
    }

    public void RemoveTimer(SceneTreeTimer timer)
    {
        timers.Remove(timer);
    }
}