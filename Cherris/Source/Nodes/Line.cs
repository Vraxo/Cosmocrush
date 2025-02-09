namespace Cherris;

public class Line : Node2D
{
    public Color Color { get; set; } = Color.White;
    public float Thickness { get; set; } = 1;

    private readonly List<Vector2> points = [];

    public override void Draw()
    {
        if (points.Count < 2)
        {
            return;
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(
                points[i],
                points[i + 1],
                Thickness,
                Color);
        }
    }

    public void AddPoint(Vector2 point)
    {
        points.Add(point);
    }

    public void ClearPoints()
    {
        points.Clear();
    }
}