namespace Nodica;

public class NavigationAgent : Node2D
{
    public Vector2 TargetPosition { get; set; } = new(0, 0);
    public List<Vector2> Path = [new()];

    private static float CellSize => NavigationManager.Instance.CellSize;
    private static int GridWidth => NavigationManager.Instance.GridWidth;
    private static int GridHeight => NavigationManager.Instance.GridHeight;

    public override void Update()
    {
        base.Update();
        UpdatePath();
    }

    protected override void Draw()
    {
        DrawGrid();
        DrawObstacles();
        DrawTarget();
        DrawPath();
    }

    private void UpdatePath()
    {
        Vector2 start = GlobalPosition;
        Vector2 goal = AdjustTargetPosition(TargetPosition);

        Vector2 startGrid = GetGridPosition(start);
        Vector2 goalGrid = GetGridPosition(goal);

        List<Vector2> openList = new() { startGrid };
        Dictionary<Vector2, Vector2> cameFrom = new();
        Dictionary<Vector2, float> gScore = new() { [startGrid] = 0 };
        Dictionary<Vector2, float> fScore = new() { [startGrid] = Heuristic(startGrid, goalGrid) };

        while (openList.Count > 0)
        {
            Vector2 current = GetLowestFScoreNode(openList, fScore);

            if (current == goalGrid)
            {
                ReconstructPath(cameFrom, current);
                break;
            }

            openList.Remove(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!NavigationManager.Instance.IsCellWalkable((int)neighbor.X, (int)neighbor.Y)) continue;

                float tentativeGScore = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goalGrid);

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
    }

    private void ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
    {
        Path.Clear();
        while (cameFrom.ContainsKey(current))
        {
            Path.Add((current * CellSize) + new Vector2(CellSize / 2, CellSize / 2));
            current = cameFrom[current];
        }
        Path.Reverse();
    }

    private void DrawGrid()
    {
        for (float x = 0; x < Window.Size.X; x += CellSize)
            DrawLine(new(x, 0), new(x, Window.Size.Y), Color.White);

        for (float y = 0; y < Window.Size.Y; y += CellSize)
            DrawLine(new(0, y), new(Window.Size.X, y), Color.White);
    }

    private void DrawObstacles()
    {
        for (int x = 0; x < GridWidth / CellSize; x++)
        {
            for (int y = 0; y < GridHeight / CellSize; y++)
            {
                if (!NavigationManager.Instance.IsCellWalkable(x, y))
                {
                    Vector2 cellPosition = new(x * CellSize, y * CellSize);
                    DrawRectangle(cellPosition, new(CellSize, CellSize), Color.Red);
                }
            }
        }
    }

    private void DrawTarget()
    {
        DrawCircle(TargetPosition, 5, Color.Red);
    }

    private void DrawPath()
    {
        for (int i = 0; i < Path.Count - 1; i++)
        {
            Vector2 start = Path[i];
            Vector2 end = Path[i + 1];
            DrawLine(start, end, Color.Red);
        }
    }

    private static List<Vector2> GetNeighbors(Vector2 node)
    {
        return new List<Vector2>
        {
            node + new Vector2(1, 0),
            node + new Vector2(-1, 0),
            node + new Vector2(0, 1),
            node + new Vector2(0, -1)
        }.Where(n => n.X >= 0 && n.Y >= 0).ToList();
    }

    private static Vector2 GetLowestFScoreNode(List<Vector2> openList, Dictionary<Vector2, float> fScore)
    {
        return openList.OrderBy(v => fScore.ContainsKey(v) ? fScore[v] : float.MaxValue).First();
    }

    private static Vector2 AdjustTargetPosition(Vector2 target)
    {
        Vector2 targetGrid = GetGridPosition(target);

        if (NavigationManager.Instance.IsCellWalkable((int)targetGrid.X, (int)targetGrid.Y))
            return target;

        return FindNearbyWalkablePosition(targetGrid);
    }

    private static Vector2 GetGridPosition(Vector2 position)
    {
        return new(MathF.Floor(position.X / NavigationManager.Instance.CellSize), MathF.Floor(position.Y / NavigationManager.Instance.CellSize));
    }

    private static Vector2 FindNearbyWalkablePosition(Vector2 targetGrid)
    {
        for (int radius = 1; radius <= 10; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2 neighbor = targetGrid + new Vector2(dx, dy);
                    if (NavigationManager.Instance.IsCellWalkable((int)neighbor.X, (int)neighbor.Y))
                        return (neighbor * NavigationManager.Instance.CellSize) + new Vector2(NavigationManager.Instance.CellSize / 2, NavigationManager.Instance.CellSize / 2);
                }
            }
        }
        return targetGrid * NavigationManager.Instance.CellSize;
    }

    private static float Heuristic(Vector2 a, Vector2 b)
    {
        return MathF.Abs(a.X - b.X) + MathF.Abs(a.Y - b.Y);
    }
}