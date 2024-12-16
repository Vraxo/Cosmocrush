namespace Nodica;

public abstract class VisualItem : Node
{
    public bool Visible { get; set; } = true;
    public bool ReadyForVisibility { get; private set; } = false;

    public override void Update()
    {
        base.Update();

        if (Visible && ReadyForVisibility)
        {
            Draw();
        }

        ReadyForVisibility = true;
    }

    protected virtual void Draw() { }

    protected void DrawCircleOutline(Vector2 position, float radius, Color color)
    {
        App.Instance.Backend.Drawing.DrawCircleOutline(position, radius, color);
    }

    protected void DrawRectangleOutline(Vector2 position, Vector2 size, Color color)
    {
        App.Instance.Backend.Drawing.DrawRectangleOutline(position, size, color);
    }

    protected void DrawRectangle(Vector2 position, Vector2 size, Color color)
    {
        App.Instance.Backend.Drawing.DrawRectangle(position, size, color);
    }

    protected void DrawRoundedRectangle(Vector2 position, Vector2 size, float roundness, int segments, Color color)
    {
        App.Instance.Backend.Drawing.DrawRoundedRectangle(position, size, roundness, segments, color);
    }

    protected void DrawThemedRectangle(Vector2 position, Vector2 size, BoxTheme theme)
    {
        float top = theme.BorderLengthUp;
        float right = theme.BorderLengthRight;
        float bottom = theme.BorderLengthDown;
        float left = theme.BorderLengthLeft;

        Vector2 outerRectanglePosition = new(position.X - left, position.Y - top);
        Vector2 outerRectangleSize = new(size.X + left + right, size.Y + top + bottom);

        if (top > 0 || right > 0 || bottom > 0 || left > 0)
        {
            DrawRoundedRectangle(
                outerRectanglePosition,
                outerRectangleSize,
                theme.Roundness,
                (int)size.Y,
                theme.BorderColor);
        }

        DrawRoundedRectangle(
            position,
            size,
            theme.Roundness,
            (int)size.Y,
            theme.FillColor);
    }

    protected static void DrawTexture(Texture texture, Vector2 position, float rotation, Vector2 scale, Color tint)
    {
        App.Instance.Backend.Drawing.DrawTexture(texture, position, rotation, scale, tint);
    }

    protected void DrawTextureScaled(Texture texture, Vector2 position, Vector2 origin, float rotation, Vector2 scale, bool flipH = false, bool flipV = false)
    {
        App.Instance.Backend.Drawing.DrawTextureScaled(texture, position, origin, rotation, scale, flipH, flipV);
    }

    protected void DrawText(string text, Vector2 position, Font font, float fontSize, float spacing, Color color)
    {
        App.Instance.Backend.Drawing.DrawText(text, position, font, fontSize, spacing, color);
    }

    protected void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        App.Instance.Backend.Drawing.DrawLine(from, to, color);
    }

    protected void DrawCircle(Vector2 position, float radius, Color color)
    {
        App.Instance.Backend.Drawing.DrawCircle(position, radius, color);
    }

    protected void DrawGrid(Vector2 size, float cellSize, Color color)
    {
        for (float x = 0; x < size.X; x += cellSize)
        {
            DrawLine(new(x, 0), new(x, size.Y), color);
        }

        for (float y = 0; y < size.Y; y += cellSize)
        {
            DrawLine(new(0, y), new(size.X, y), color);
        }
    }
}