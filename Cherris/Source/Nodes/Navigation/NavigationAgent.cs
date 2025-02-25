namespace Cherris;

public class NavigationAgent : Node2D
{
    public Vector2 TargetPosition { get; set; } = new(0, 0);
    public List<Vector2> Path = [new()];
    public NavigationRegion? Region { get; set; }
    public Color Color { get; set; } = new(255, 0, 0, 255);

    private Vector2 lastTargetPosition;
    private Vector2 lastStartPosition;

    private float CellSize => Region?.CellSize ?? 32f;

    private const float TargetMoveThreshold = 1.0f;
    private const float StartMoveThreshold = 1.0f;

    // Main

    public override void Process()
    {
        base.Process();

        Vector2 currentStart = GlobalPosition;
        bool targetMoved = TargetPosition.DistanceTo(lastTargetPosition) > TargetMoveThreshold;
        bool startMoved = currentStart.DistanceTo(lastStartPosition) > StartMoveThreshold;

        if (Region is null || !targetMoved && !startMoved)
        {
            return;
        }

        UpdatePath();
        lastTargetPosition = TargetPosition;
        lastStartPosition = currentStart;
    }

    // Draw

    public override void Draw()
    {
        if (Region is null)
        {
            return;
        }

        DrawTarget();
        DrawPath();
    }

    private void DrawTarget()
    {
        DrawCircle(
            TargetPosition,
            5,
            Color);
    }

    private void DrawPath()
    {
        for (int i = 0; i < Path.Count - 1; i++)
        {
            Vector2 start = Path[i];
            Vector2 end = Path[i + 1];

            DrawDashedLine(
                start,
                end,
                4,
                8,
                2,
                Color.Red);
        }
    }

    // Pathetic

    private void UpdatePath()
    {
        Vector2 start = GlobalPosition;
        Vector2 goal = AdjustTargetPosition(TargetPosition);

        Vector2 startGrid = GetGridPosition(start);
        Vector2 goalGrid = GetGridPosition(goal);

        List<Vector2> openList = [startGrid];
        Dictionary<Vector2, Vector2> cameFrom = [];
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
                if (!Region!.IsCellWalkable((int)neighbor.X, (int)neighbor.Y))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + 1;

                if (gScore.TryGetValue(neighbor, out float value) && tentativeGScore >= value)
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                value = tentativeGScore;
                gScore[neighbor] = value;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goalGrid);

                if (openList.Contains(neighbor))
                {
                    continue;
                }

                openList.Add(neighbor);
            }
        }
    }

    private void ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
    {
        Path.Clear();

        while (cameFrom.ContainsKey(current))
        {
            Path.Add((current * CellSize) + new Vector2(CellSize / 2));
            current = cameFrom[current];
        }

        Path.Reverse();
    }

    // Other

    private static List<Vector2> GetNeighbors(Vector2 node)
    {
        return new[]
        {
            // Cardinal directions
            node + new Vector2(1, 0),
            node + new Vector2(-1, 0),
            node + new Vector2(0, 1),
            node + new Vector2(0, -1),
            // Diagonal directions
            node + new Vector2(1, 1),
            node + new Vector2(-1, -1),
            node + new Vector2(1, -1),
            node + new Vector2(-1, 1)
        }
        .Where(n => n.X >= 0 && n.Y >= 0)
        .ToList();
    }

    private static Vector2 GetLowestFScoreNode(List<Vector2> openList, Dictionary<Vector2, float> fScore)
    {
        return openList.OrderBy(v => fScore.TryGetValue(v, out float value) ? value : float.MaxValue).First();
    }

    private static Vector2 GetGridPosition(Vector2 position)
    {
        return new(float.Floor(position.X / 32f), float.Floor(position.Y / 32f));
    }

    private static float Heuristic(Vector2 a, Vector2 b)
    {
        var dx = float.Abs(a.X - b.X);
        var dy = float.Abs(a.Y - b.Y);
        return float.Max(dx, dy); // Chebyshev distance for 8-direction movement
    }

    private Vector2 AdjustTargetPosition(Vector2 target)
    {
        Vector2 targetGrid = GetGridPosition(target);

        if (Region?.IsCellWalkable((int)targetGrid.X, (int)targetGrid.Y) ?? false)
        {
            return target;
        }

        return FindNearbyWalkablePosition(targetGrid);
    }
    
    private Vector2 FindNearbyWalkablePosition(Vector2 targetGrid)
    {
        for (int radius = 1; radius <= 10; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2 neighbor = targetGrid + new Vector2(dx, dy);

                    if (!(Region?.IsCellWalkable((int)neighbor.X, (int)neighbor.Y) ?? false))
                    {
                        continue;
                    }

                    return (neighbor * 32f) + new Vector2(16f, 16f);
                }
            }
        }
        return targetGrid * 32f;
    }
}