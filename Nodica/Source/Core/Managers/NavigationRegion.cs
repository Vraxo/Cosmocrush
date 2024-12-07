namespace Nodica;

public class NavigationRegion : Node2D
{
    public int Width { get; set; } = 1024;
    public int Height { get; set; } = 768;
    public float CellSize { get; set; } = 32f;

    private readonly bool[,] grid;

    public NavigationRegion()
    {
        grid = new bool[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                grid[x, y] = true;  // All cells are walkable by default
            }
        }
    }

    public bool IsCellWalkable(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height && grid[x, y];
    }

    public void MarkUnwalkable(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            grid[x, y] = false;
        }
    }

    public void MarkWalkable(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            grid[x, y] = true;
        }
    }

    public override void Ready()
    {
        NavigationManager.Instance.RegisterRegion(this);
    }

    public override void Destroy()
    {
        NavigationManager.Instance.UnregisterRegion(this);
    }

    protected override void Draw()
    {
        for (float x = 0; x < Width; x += CellSize)
            DrawLine(new(x, 0), new(x, Height), Color.White);

        for (float y = 0; y < Height; y += CellSize)
            DrawLine(new(0, y), new(Width, y), Color.White);

        for (int x = 0; x < Width / CellSize; x++)
        {
            for (int y = 0; y < Height / CellSize; y++)
            {
                if (!IsCellWalkable(x, y))
                {
                    Vector2 cellPosition = new(x * CellSize, y * CellSize);
                    DrawRectangle(cellPosition, new(CellSize, CellSize), Color.Red);
                }
            }
        }
    }
}
