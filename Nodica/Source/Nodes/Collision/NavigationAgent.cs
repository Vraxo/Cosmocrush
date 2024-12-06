namespace Nodica;

public class NavigationAgent : Node2D
{
    public Vector2 TargetPosition { get; set; } = new(0, 0);
    public List<Vector2> Path = [new()];

    private static float Heuristic(Vector2 a, Vector2 b)
        => MathF.Abs(a.X - b.X) + MathF.Abs(a.Y - b.Y);

    private static Vector2 AdjustTargetPosition(Vector2 target)
    {
        Vector2 targetGrid = new(MathF.Floor(target.X / NavigationManager.Instance.CellSize), MathF.Floor(target.Y / NavigationManager.Instance.CellSize));

        if (NavigationManager.Instance.IsCellWalkable((int)targetGrid.X, (int)targetGrid.Y))
            return target;

        for (int radius = 1; radius <= 10; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2 neighbor = targetGrid + new Vector2(dx, dy);
                    if (NavigationManager.Instance.IsCellWalkable((int)neighbor.X, (int)neighbor.Y))
                    {
                        return (neighbor * NavigationManager.Instance.CellSize) + new Vector2(NavigationManager.Instance.CellSize / 2, NavigationManager.Instance.CellSize / 2);
                    }
                }
            }
        }

        return target;
    }

    // A* Pathfinding algorithm
    private void UpdatePath()
    {
        Vector2 start = GlobalPosition;
        Vector2 goal = AdjustTargetPosition(TargetPosition);

        Vector2 startGrid = new(MathF.Floor(start.X / NavigationManager.Instance.CellSize), MathF.Floor(start.Y / NavigationManager.Instance.CellSize));
        Vector2 goalGrid = new(MathF.Floor(goal.X / NavigationManager.Instance.CellSize), MathF.Floor(goal.Y / NavigationManager.Instance.CellSize));

        List<Vector2> openList = new() { startGrid };
        Dictionary<Vector2, Vector2> cameFrom = [];
        Dictionary<Vector2, float> gScore = [];
        Dictionary<Vector2, float> fScore = [];

        gScore[startGrid] = 0;
        fScore[startGrid] = Heuristic(startGrid, goalGrid);

        while (openList.Count > 0)
        {
            Vector2 current = openList.OrderBy(v => fScore.ContainsKey(v) ? fScore[v] : float.MaxValue).First();

            if (current == goalGrid)
            {
                Path.Clear();
                while (cameFrom.ContainsKey(current))
                {
                    Path.Add((current * NavigationManager.Instance.CellSize) + new Vector2(NavigationManager.Instance.CellSize / 2, NavigationManager.Instance.CellSize / 2));
                    current = cameFrom[current];
                }
                Path.Reverse();
                break;
            }

            openList.Remove(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!NavigationManager.Instance.IsCellWalkable((int)neighbor.X, (int)neighbor.Y))
                    continue;

                float tentativeGScore = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goalGrid);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
    }

    private List<Vector2> GetNeighbors(Vector2 node)
    {
        List<Vector2> neighbors =
        [
            node + new Vector2(1, 0),
            node + new Vector2(-1, 0),
            node + new Vector2(0, 1),
            node + new Vector2(0, -1)
        ];

        return neighbors.Where(n => n.X >= 0 && n.Y >= 0).ToList();
    }

    public override void Update()
    {
        base.Update();

        UpdatePath();
    }

    protected override void Draw()
    {
        for (float x = 0; x < Window.Size.X; x += NavigationManager.Instance.CellSize)
        {
            DrawLine(new(x, 0), new(x, Window.Size.Y), Color.White);
        }

        for (float y = 0; y < Window.Size.Y; y += NavigationManager.Instance.CellSize)
        {
            DrawLine(new(0, y), new(Window.Size.X, y), Color.White);
        }

        for (int x = 0; x < NavigationManager.Instance.GridWidth / NavigationManager.Instance.CellSize; x++)
        {
            for (int y = 0; y < NavigationManager.Instance.GridHeight / NavigationManager.Instance.CellSize; y++)
            {
                if (!NavigationManager.Instance.IsCellWalkable(x, y))
                {
                    Vector2 cellPosition = new(x * NavigationManager.Instance.CellSize, y * NavigationManager.Instance.CellSize);
                    DrawRectangle(cellPosition, new(NavigationManager.Instance.CellSize, NavigationManager.Instance.CellSize), Color.Red);
                }
            }
        }

        DrawCircle(TargetPosition, 5, Color.Red);

        for (int i = 0; i < Path.Count - 1; i++)
        {
            Vector2 start = Path[i];
            Vector2 end = Path[i + 1];
            DrawLine(start, end, Color.Red);
        }
    }
}