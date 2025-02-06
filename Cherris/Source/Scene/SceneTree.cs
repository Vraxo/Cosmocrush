namespace Cherris;

public sealed class SceneTree
{
    private static SceneTree? _instance;
    public static SceneTree Instance => _instance ??= new();

    public Node? RootNode { get; set; }

    private readonly List<SceneTreeTimer> timers = [];

    // Main

    private SceneTree() { }

    public void Process()
    {
        RootNode?.Process();

        List<SceneTreeTimer> timersToProces = new(timers);

        foreach (SceneTreeTimer timer in timersToProces)
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