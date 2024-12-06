namespace Nodica;

public sealed class NavigationManager
{
    private static NavigationManager? _instance;
    public static NavigationManager Instance => _instance ??= new();

    public int GridWidth = 1024;
    public int GridHeight = 768;
    public float CellSize = 32f;

    private readonly bool[,] grid = new bool[1024, 768];

    public List<NavigationObstacle> Obstacles = [];

    private NavigationManager()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                grid[x, y] = true;
            }
        }
    }

    public void RegisterObstacle(NavigationObstacle obstacle)
    {
        Obstacles.Add(obstacle);

        Vector2 start = obstacle.GlobalPosition - obstacle.Origin;
        Vector2 end = start + obstacle.Size;

        int startX = (int)Math.Floor(start.X / CellSize);
        int startY = (int)Math.Floor(start.Y / CellSize);
        int endX = (int)Math.Ceiling(end.X / CellSize);
        int endY = (int)Math.Ceiling(end.Y / CellSize);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                MarkUnwalkable(x, y);
            }
        }
    }

    public void UnregisterObstacle(NavigationObstacle obstacle)
    {
        Obstacles.Remove(obstacle);

        Vector2 start = obstacle.GlobalPosition;
        Vector2 end = start + obstacle.Size;

        for (float x = start.X; x < end.X; x += CellSize)
        {
            for (float y = start.Y; y < end.Y; y += CellSize)
            {
                Vector2 gridPosition = new(x / CellSize, y / CellSize);
                MarkWalkable((int)gridPosition.X, (int)gridPosition.Y);
            }
        }
    }

    public void MarkUnwalkable(int x, int y)
    {
        if (x > 0 && x < GridWidth && y > 0 && y < GridHeight)
        {
            grid[x, y] = false;
        }
    }

    public void MarkWalkable(int x, int y)
    {
        if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight)
        {
            grid[x, y] = true;
        }
    }

    public bool IsCellWalkable(int x, int y)
    {
        return x >= 0 && x < GridWidth && y >= 0 && y < GridHeight && grid[x, y];
    }
}