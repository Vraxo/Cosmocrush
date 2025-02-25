﻿using Raylib_cs;

namespace Cherris;

public abstract class VisualItem : Node
{
    public Shader? Shader { get; set; }
    public bool ReadyForVisibility { get; set; } = false;

    public bool Visible 
    {
        get; 
        
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            VisibleChanged?.Invoke(this, Visible);
        }
    } = true;

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
            LayerChanged?.Invoke(this, Layer);
        }
    } = 0;

    // Events

    public delegate void VisibleEvent(VisualItem sender, bool visible);
    public delegate void LayerEvent(VisualItem sender, int layer);

    public event VisibleEvent? VisibleChanged;
    public event LayerEvent? LayerChanged;

    // Main

    public virtual void Draw() { }

    // Circle

    public void DrawCircle(Vector2 position, float radius, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawCircleV(
                position,
                radius,
                color);
        }, Layer);
    }

    public void DrawCircleOutline(Vector2 position, float radius, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawCircleLinesV(
                position,
                radius,
                color);
        }, Layer);
    }

    // Pixel

    public void DrawPixel(Vector2 position, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawPixelV(
                position,
                color);
        }, Layer);
    }

    // Rectangle

    public void DrawRectangle(Vector2 position, Vector2 size, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawRectangleV(
                position,
                size,
                color);
        }, Layer);
    }

    public void DrawRectangleOutline(Vector2 position, Vector2 size, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawRectangleLines(
                (int)position.X,
                (int)position.Y,
                (int)size.X,
                (int)size.Y,
                color);
        }, Layer);
    }

    public void DrawRectangleRounded(Vector2 position, Vector2 size, float roundness, int segments, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawRectangleRounded(
                new()
                {
                    Position = position,
                    Size = size
                },
                roundness,
                segments,
                color);
        }, Layer);
    }

    public void DrawRectangleThemed(Vector2 position, Vector2 size, BoxStyle theme)
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

    public void DrawRectangleOutlineRounded(Vector2 position, Vector2 size, float roundness, int segments, float thickness, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Rectangle rectangle = new()
            {
                Position = position,
                Size = size
            };

            Raylib.DrawRectangleRoundedLines(
                rectangle,
                roundness,
                segments,
                color);
        }, Layer);
    }

    // Same as V1, except the outline is moved down.

    /*public void DrawRectangleThemed(Vector2 position, Vector2 size, BoxStyle theme)
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
    //public void DrawRectangleThemed(Vector2 position, Vector2 size, BoxStyle theme)
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

    public void DrawTexture(Texture texture, Vector2 position, float rotation, Vector2 scale, Color tint)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawTextureEx(
                texture,
                position,
                rotation,
                scale.Length(),
                tint);
        }, Layer);
    }

    public void DrawTextureScaled(Texture texture, Vector2 position, Vector2 origin, float rotation, Vector2 scale, bool flipH = false, bool flipV = false)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawTexturePro(
                texture,
                new()
                {
                    Position = new(0, 0),
                    Width = texture.Size.X * (flipH ? -1 : 1),
                    Height = texture.Size.Y * (flipV ? -1 : 1),
                },
                new()
                {
                    Position = position,
                    Size = texture.Size * scale
                },
                origin,
                rotation,
                Color.White);

        }, Layer);
    }

    public void DrawTexturedRectangle(Texture texture, Rectangle source, Rectangle target, Vector2 origin, float rotation)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawTexturePro(
                texture,
                source,
                target,
                origin,
                rotation,
                Color.White);

        }, Layer);
    }

    // Text

    public void DrawText(string text, Vector2 position, Font font, float fontSize, float spacing, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawTextEx(
                font,
                text,
                position,
                fontSize,
                spacing,
                color
            );
        }, Layer);
    }

    public void DrawTextOutlined(string text, Vector2 position, Font font, float fontSize, float spacing, Color color, float outlineSize, Color outlineColor)
    {
        RenderServer.Instance.Submit(() =>
        {
            if (outlineSize > 0)
            {
                for (int x = (int)-outlineSize; x <= outlineSize; x++)
                {
                    for (int y = (int)-outlineSize; y <= outlineSize; y++)
                    {
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }

                        Raylib.DrawTextEx(
                            font,
                            text,
                            position + new Vector2(x, y),
                            fontSize,
                            spacing,
                            outlineColor
                        );
                    }
                }
            }

            Raylib.DrawTextEx(
                font,
                text,
                position,
                fontSize,
                spacing,
                color
            );
        }, Layer);
    }

    // Line

    public void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
    {
        RenderServer.Instance.Submit(() =>
        {
            Raylib.DrawLineEx(
                start,
                end,
                thickness,
                color);
        }, Layer);
    }

    public void DrawGrid(Vector2 size, float cellSize, Color color)
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

    public void DrawDashedLine(Vector2 start, Vector2 end, float dashLength, float gapLength, float thickness, Color color)
    {
        Vector2 direction = end - start;
        float totalLength = direction.Length();

        if (totalLength <= 0 || dashLength <= 0 || gapLength <= 0)
            return;

        direction = Vector2.Normalize(direction);

        float currentPosition = 0f;
        bool drawingDash = true;

        while (currentPosition < totalLength)
        {
            if (drawingDash)
            {
                // Calculate dash segment
                float remaining = totalLength - currentPosition;
                float segmentLength = Math.Min(dashLength, remaining);

                Vector2 segmentStart = start + direction * currentPosition;
                Vector2 segmentEnd = segmentStart + direction * segmentLength;

                // Draw the dash segment
                DrawLine(segmentStart, segmentEnd, thickness, color);

                currentPosition += segmentLength;
                drawingDash = false;
            }
            else
            {
                // Skip gap
                currentPosition += gapLength;
                drawingDash = true;
            }
        }
    }

    // Shader

    public void SetShaderValue(int loc, float[] values, ShaderUniformDataType uniformType)
    {
        if (Shader is null)
        {
            Log.Error("Shader is null.");
            return;
        }

        Shader.SetValue(loc, values, uniformType);
    }

    public int GetShaderLocation(string uniformName)
    {
        if (Shader is null)
        {
            Log.Error("Shader is null.");
            return 0;
        }

        return Shader.GetLocation(uniformName);
    }
}