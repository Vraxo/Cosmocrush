﻿using Cherris.DrawCommands;
using Raylib_cs;

namespace Cherris;

public abstract class VisualItem : Node
{
    public Shader Shader { get; set; }
    public bool UseShader { get; set; } = false;
    public bool ReadyForVisibility { get; set; } = false;

    private bool _visible = true;
    public bool Visible 
    {
        get => _visible; 
        
        set
        {
            if (_visible == value)
            {
                return;
            }

            _visible = value;
            VisibleChanged?.Invoke(this, _visible);
        }
    }

    public int Layer
    {
        get;

        set
        {
            if (field == value)
            {
                return;
            }
            
            field = value;
            LayerChanged?.Invoke(this, field);
        }
    } = 0;

    public delegate void VisualItemVisibleEventHandler(VisualItem sender, bool visible);
    public delegate void VisualItemLayerEventHandler(VisualItem sender, int layer);

    public event VisualItemVisibleEventHandler? VisibleChanged;
    public event VisualItemLayerEventHandler? LayerChanged;

    // Main

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

    // Circle

    protected void DrawCircle(Vector2 position, float radius, Color color)
    {
        CircleDC circle = new()
        {
            Position = position,
            Radius = radius,
            Color = color,
            Layer = Layer
        };

        RenderServer.Instance.Submit(circle);
    }

    protected void DrawCircleOutline(Vector2 position, float radius, Color color)
    {
        CircleDC circleOutline = new()
        {
            Position = position,
            Radius = radius,
            Color = color,
            Layer = Layer
        };

        RenderServer.Instance.Submit(circleOutline);
    }

    // Pixel

    protected void DrawPixel(Vector2 position, Color color)
    {
        PixelDC pixel = new()
        {
            Position = position,
            Color = color,
            Layer = Layer
        };

        RenderServer.Instance.Submit(pixel);
    }

    // Rectangle

    protected void DrawRectangle(Vector2 position, Vector2 size, Color color)
    {
        RectangleDC rectangle = new()
        {
            Position = position,
            Size = size,
            Color = color,
            Layer = Layer
        };

        RenderServer.Instance.Submit(rectangle);
    }

    protected void DrawRectangleOutline(Vector2 position, Vector2 size, Color color)
    {
        RectangleOutlineDC rectangleOutline = new()
        {
            Position = position,
            Size = size,
            Color = color,
            Layer = Layer
        };

        RenderServer.Instance.Submit(rectangleOutline);
    }

    protected void DrawRectangleRounded(Vector2 position, Vector2 size, float roundness, int segments, Color color)
    {
        RectangleRoundedDC roundedRectangle = new()
        {
            Position = position,
            Size = size,
            Roundness = roundness,
            Segments = segments,
            Color = color,
            Layer = Layer
        };

        RenderServer.Instance.Submit(roundedRectangle);
    }

    protected void DrawRectangleThemed(Vector2 position, Vector2 size, BoxTheme theme)
    {
        // Border lengths for each side
        float top = theme.BorderLengthTop;
        float right = theme.BorderLengthRight;
        float bottom = theme.BorderLengthBottom;
        float left = theme.BorderLengthLeft;

        // Adjust the positions for borders to avoid visual artifacts.
        Vector2 outerRectanglePosition = position;
        Vector2 outerRectangleSize = size;

        // Check if we need to adjust the borders
        if (top > 0 || right > 0 || bottom > 0 || left > 0)
        {
            // We adjust the border size for the outer rectangle
            outerRectanglePosition = new(position.X - left + 1, position.Y - top + 1);
            outerRectangleSize = new(size.X + left + right - 2, size.Y + top + bottom - 2);

            // Draw the border (only where needed)
            DrawRectangleOutlineRounded(
                outerRectanglePosition,
                outerRectangleSize,
                theme.Roundness,
                (int)size.Y,
                1,
                theme.BorderColor);
        }

        // Draw the inner rectangle (the actual filled area of the progress bar)
        DrawRectangleRounded(
            position,
            size,
            theme.Roundness,
            (int)size.Y,
            theme.FillColor);
    }

    protected void DrawRectangleOutlineRounded(Vector2 position, Vector2 size, float roundness, int segments, float thickness, Color color)
    {
        RectangleOutlineRoundedDC roundedRectangle = new()
        {
            Position = position,
            Size = size,
            Roundness = roundness,
            Segments = segments,
            Thickness = thickness,
            Color = color,
            Layer = Layer
        };

        RenderServer.Instance.Submit(roundedRectangle);
    }

    // Same as V1, except the outline is moved down.

    /*protected void DrawRectangleThemed(Vector2 position, Vector2 size, BoxTheme theme)
    //{
    //    float top = theme.BorderLengthTop;
    //    float right = theme.BorderLengthRight;
    //    float bottom = theme.BorderLengthBottom;
    //    float left = theme.BorderLengthLeft;
    //
    //    Vector2 outerRectanglePosition = new(position.X - left, position.Y - top + 10);
    //    Vector2 outerRectangleSize = new(size.X + left + right, size.Y + top + bottom - 10);
    //
    //    if (top > 0 || right > 0 || bottom > 0 || left > 0)
    //    {
    //        DrawRectangleRounded(
    //            outerRectanglePosition,
    //            outerRectangleSize,
    //            theme.Roundness,
    //            (int)size.Y,
    //            theme.BorderColor);
    //    }
    //
    //    DrawRectangleRounded(
    //        position,
    //        size,
    //        theme.Roundness,
    //        (int)size.Y,
    //        theme.FillColor);
    //}

    // V1 - Has artifacts.
    //protected void DrawRectangleThemed(Vector2 position, Vector2 size, BoxTheme theme)
    //{
    //    float top = theme.BorderLengthTop;
    //    float right = theme.BorderLengthRight;
    //    float bottom = theme.BorderLengthBottom;
    //    float left = theme.BorderLengthLeft;
    //
    //    if (top > 0)
    //    {
    //        Vector2 borderPosition = new(position.X - left, position.Y - top);
    //        Vector2 borderSize = new(size.X + left + right, top);
    //        DrawRectangleRounded(
    //            borderPosition,
    //            borderSize,
    //            theme.Roundness,
    //            (int)size.Y,
    //            theme.BorderColor);
    //    }
    //
    //    if (right > 0)
    //    {
    //        Vector2 borderPosition = new(position.X + size.X, position.Y - top);
    //        Vector2 borderSize = new(right, size.Y + top + bottom);
    //        DrawRectangleRounded(
    //             borderPosition,
    //            borderSize,
    //            theme.Roundness,
    //             (int)size.Y,
    //            theme.BorderColor);
    //    }
    //
    //
    //    if (bottom > 0)
    //    {
    //        Vector2 borderPosition = new(position.X - left, position.Y + size.Y);
    //        Vector2 borderSize = new(size.X + left + right, bottom);
    //        DrawRectangleRounded(
    //            borderPosition,
    //           borderSize,
    //            theme.Roundness,
    //            (int)size.Y,
    //            theme.BorderColor);
    //    }
    //
    //    if (left > 0)
    //    {
    //        Vector2 borderPosition = new(position.X - left, position.Y - top);
    //        Vector2 borderSize = new(left, size.Y + top + bottom);
    //        DrawRectangleRounded(
    //           borderPosition,
    //           borderSize,
    //           theme.Roundness,
    //           (int)size.Y,
    //           theme.BorderColor);
    //    }
    //
    //
    //    DrawRectangleRounded(
    //        position,
    //        size,
    //        theme.Roundness,
    //        (int)size.Y,
    //        theme.FillColor);
    //}
    */

    // Texture

    protected void DrawTexture(Texture texture, Vector2 position, float rotation, Vector2 scale, Color tint)
    {
        TextureDC textureDrawCommand = new()
        {
            Texture = texture,
            Position = position,
            Rotation = rotation,
            Scale = scale,
            Tint = tint,
            Layer = Layer
        };

        RenderServer.Instance.Submit(textureDrawCommand);
    }

    protected void DrawTextureScaled(Texture texture, Vector2 position, Vector2 origin, float rotation, Vector2 scale, bool flipH = false, bool flipV = false)
    {
        TextureScaledDC textureDrawCommand = new()
        {
            Texture = texture,
            Position = position,
            Origin = origin,
            Rotation = rotation,
            Scale = scale,
            FlipH = flipH,
            FlipV = flipV,
            Layer = Layer,
            Shader = Shader,
            UseShader = UseShader
        };

        RenderServer.Instance.Submit(textureDrawCommand);
    }

    protected void DrawTexturedRectangle(Texture texture, Rectangle source, Rectangle target, Vector2 origin, float rotation)
    {
        TexturedRectangleDC texturedRectangle = new()
        {
            Texture = texture,
            Source = source,
            Target = target,
            Origin = origin,
            Rotation = rotation,
            Layer = Layer
        };

        RenderServer.Instance.Submit(texturedRectangle);
    }

    // Text

    protected void DrawText(string text, Vector2 position, Font font, float fontSize, float spacing, Color color)
    {
        TextDC textDC = new()
        {
            Text = text,
            Position = position,
            Font = font,
            FontSize = fontSize,
            Spacing = spacing,
            Color = color,
            Layer = Layer
        };

        RenderServer.Instance.Submit(textDC);
    }

    protected void DrawTextOutlined(string text, Vector2 position, Font font, float fontSize, float spacing, Color color, float outlineThickness, Color outlineColor)
    {
        TextDC textDC = new()
        {
            Text = text,
            Position = position,
            Font = font,
            FontSize = fontSize,
            Spacing = spacing,
            Color = color,
            OutlineSize = outlineThickness,
            OutlineColor = outlineColor,
            Layer = Layer
        };

        RenderServer.Instance.Submit(textDC);
    }

    // Line

    protected void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
    {
        LineDC lineDC = new()
        {
            Start = start,
            End = end,
            Color = color,
            Thickness = thickness,
            Layer = Layer
        };

        RenderServer.Instance.Submit(lineDC);
    }

    protected void DrawGrid(Vector2 size, float cellSize, Color color)
    {
        for (float x = 0; x < size.X; x += cellSize)
        {
            DrawLine(new(x, 0), new(x, size.Y), 1, color);
        }

        for (float y = 0; y < size.Y; y += cellSize)
        {
            DrawLine(new(0, y), new(size.X, y), 1, color);
        }
    }
}