namespace Cherris;

public sealed class NavigationServer
{
    public static NavigationServer Instance { get; } = new();

    public List<NavigationRegion> Regions = [];

    private NavigationServer() { }

    public void RegisterRegion(NavigationRegion region)
    {
        if (!Regions.Contains(region))
        {
            Regions.Add(region);
        }
    }

    public void UnregisterRegion(NavigationRegion region)
    {
        Regions.Remove(region);
    }

    public void RegisterObstacle(NavigationObstacle obstacle)
    {
        Vector2 start = obstacle.GlobalPosition - obstacle.Origin;
        Vector2 end = start + obstacle.Size * obstacle.Scale;

        foreach (var region in Regions)
        {
            var startX = (int)float.Floor(start.X / region.CellSize);
            var startY = (int)float.Floor(start.Y / region.CellSize);
            var endX = (int)float.Ceiling(end.X / region.CellSize);
            var endY = (int)float.Ceiling(end.Y / region.CellSize);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    region.MarkUnwalkable(x, y);
                }
            }
        }
    }

    public void UnregisterObstacle(NavigationObstacle obstacle)
    {
        Vector2 start = obstacle.GlobalPosition;
        Vector2 end = start + obstacle.Size;

        foreach (var region in Regions)
        {
            for (float x = start.X; x < end.X; x += region.CellSize)
            {
                for (float y = start.Y; y < end.Y; y += region.CellSize)
                {
                    Vector2 gridPosition = new(x / region.CellSize, y / region.CellSize);
                    region.MarkWalkable((int)gridPosition.X, (int)gridPosition.Y);
                }
            }
        }
    }
}