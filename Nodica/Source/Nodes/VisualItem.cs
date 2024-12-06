using Raylib_cs;

namespace Nodica;

public abstract class VisualItem : Node
{
    public bool Visible { get; set; } = true;
    public bool ReadyForVisibility { get; private set; } = false;

    public override void Update()
    {
        if (Visible && ReadyForVisibility)
        {
            Draw();
        }
        base.Update();
        ReadyForVisibility = true;
    }

    protected virtual void Draw() { }

    protected void DrawCircleOutline(Vector2 position, float radius, Color color)
    {
        Raylib.DrawCircleLinesV(
            position,
            radius,
            color);
    }

    protected void DrawRectangleOutline(Vector2 position, Vector2 size, Color color)
    {
        Raylib.DrawRectangleLines(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y,
            color);
    }

    protected void DrawRoundedRectangle(Vector2 position, Vector2 size, float roundness, int segments, Color color)
    {
        Rectangle rectangle = new()
        {
            Position = position,
            Size = size
        };

        Raylib.DrawRectangleRounded(
            rectangle,
            roundness,
            segments,
            color);
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

    protected void DrawTexture(Texture2D texture, Vector2 position, float rotation, Vector2 scale, Color tint)
    {
        Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);

        Rectangle destRect = new Rectangle(
            position.X,
            position.Y,
            texture.Width * scale.X,
            texture.Height * scale.Y);

        Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

        Raylib.DrawTexturePro(
            texture,
            sourceRect,
            destRect,
            origin,
            rotation,
            tint);
    }

    protected void DrawTextureScaled(Texture texture, Vector2 position, Vector2 origin, float rotation, Vector2 scale, bool flipH = false, bool flipV = false)
    {
        Rectangle source = new()
        {
            Position = new(0, 0),
            Width = texture.Size.X * (flipH ? -1 : 1),
            Height = texture.Size.Y * (flipV ? -1 : 1),
        };

        Rectangle destination = new()
        {
            Position = position,
            Size = texture.Size * scale
        };

        Raylib.DrawTexturePro(
            texture,
            source,
            destination,
            origin,
            rotation,
            Color.White);
    }

    protected void DrawText(string text, Vector2 position, Raylib_cs.Font font, float fontSize, float spacing, Color color)
    {
        Raylib.DrawTextEx(
            font,
            text,
            position,
            fontSize,
            spacing,
            color);
    }

    protected void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        Raylib.DrawLineV(from, to, color);
    }
}