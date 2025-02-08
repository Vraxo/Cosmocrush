namespace Cherris;

public sealed class SceneTree
{
    private static SceneTree? _instance;
    public static SceneTree Instance => _instance ??= new();

    public Node? RootNode { get; set; }
    private readonly List<SceneTreeTimer> timers = [];
    public bool Paused { get; private set; } = false;

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

        List<SceneTreeTimer> timersToProcess = new(timers);
     
        foreach (SceneTreeTimer timer in timersToProcess)
        {
            timer.Process();
        }

        if (Input.IsKeyPressed(KeyCode.Enter))
        {
            PrintTree();
        }
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