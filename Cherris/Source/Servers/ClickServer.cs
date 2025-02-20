namespace Cherris;

public sealed class ClickServer
{
    public static ClickServer Instance { get; } = new();

    public int MinLayer = -1;

    private readonly List<Clickable> clickables = [];
    private const bool debug = false;

    private ClickServer() { }

    public void Register(Clickable clickable)
    {
        clickables.Add(clickable);
    }

    public void Unregister(Clickable clickable)
    {
        clickables.Remove(clickable);
    }

    public void Process()
    {
        if (Input.IsMouseButtonPressed(MouseButtonCode.Left))
        {
            SignalClick(MouseButtonCode.Left);
        }

        if (Input.IsMouseButtonPressed(MouseButtonCode.Right))
        {
            SignalClick(MouseButtonCode.Right);
        }
    }

    public int GetHighestLayer()
    {
        int highestLayer = MinLayer;

        foreach (Clickable clickable in clickables)
        {
            if (clickable.Layer > highestLayer)
            {
                highestLayer = clickable.Layer;
            }
        }

        return highestLayer;
    }

    private void SignalClick(MouseButtonCode mouseButton)
    {
        List<Clickable> viableClickables = GetViableClickables();

        if (viableClickables.Count > 0)
        {
            Clickable? topClickable = GetTopClickable(viableClickables);

            if (topClickable != null)
            {
                if (mouseButton == MouseButtonCode.Left)
                {
                    topClickable.OnTopLeft = true;
                    Log.Info($"[ClickServer] '{topClickable}' has been left clicked.", "ClickServer");
                }
                else
                {
                    topClickable.OnTopRight = true;
                    Log.Info($"[ClickServer] '{topClickable}' has been right clicked.", "ClickServer");
                }
            }
        }
    }

    private List<Clickable> GetViableClickables()
    {
        List<Clickable> viableClickables = [];

        foreach (Clickable clickable in clickables)
        {
            if (IsMouseOverNode2D(clickable))
            {
                viableClickables.Add(clickable);
            }
        }

        Log.Info($"[ClickServer] {viableClickables.Count} viable clickables.", "ClickServer");

        return viableClickables;
    }

    private Clickable? GetTopClickable(List<Clickable> viableClickables)
    {
        Clickable? topClickable = null;
        int highestLayer = MinLayer;

        foreach (Clickable clickable in viableClickables)
        {
            if (clickable.Layer >= highestLayer)
            {
                highestLayer = clickable.Layer;
                topClickable = clickable;
            }
        }

        Log.Info($"[ClickServer] The highest layer is {viableClickables.Count}.", "ClickServer");

        return topClickable;
    }

    private static bool IsMouseOverNode2D(Node2D node)
    {
        Vector2 mousePosition = Input.WorldMousePosition;

        bool isMouseOver =
            mousePosition.X > node.GlobalPosition.X - node.Origin.X &&
            mousePosition.X < node.GlobalPosition.X + node.ScaledSize.X - node.Origin.X &&
            mousePosition.Y > node.GlobalPosition.Y - node.Origin.Y &&
            mousePosition.Y < node.GlobalPosition.Y + node.ScaledSize.Y - node.Origin.Y;

        return isMouseOver;
    }
}