﻿using Raylib_cs;

namespace Cherris;

public class VerticalContainer : ClickableRectangle
{
    public float MovementSpeed { get; set; } = 16;

    public float StartY
    {
        get;

        set
        {
            float totalHeight = 0;
            float lastChildHeight = 0;

            foreach (Node child in Children)
            {
                if (child is Node2D node)
                {
                    totalHeight += node.Size.Y;
                    lastChildHeight = node.Size.Y;
                }
            }

            float minimumY = Math.Min(0, Size.Y - totalHeight - lastChildHeight);

            field = Math.Clamp(value, minimumY, 0);
        }
    } = 0;

    // Main

    public VerticalContainer()
    {
        Size = new(250, 150);
        OriginPreset = OriginPreset.TopLeft;

        HAlignment = HorizontalAlignment.Center;
        VAlignment = VerticalAlignment.Top;
    }

    public override void Process()
    {
        Raylib.BeginScissorMode(
            (int)(GlobalPosition.X - Origin.X),
            (int)(GlobalPosition.Y - Origin.Y),
            (int)Size.X,
            (int)Size.Y);

        SortChildren();
        RedirectClicksToChildren();
        base.Process();
        Raylib.EndScissorMode();
        HandleScrolling();
    }

    public override void Draw()
    {
        base.Draw();

        DrawRectangleOutline(
            GlobalPosition - Origin,
            Size,
            Color.Gray);
    }

    // utils

    private void SortChildren()
    {
        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i] is Node2D child)
            {
                child.Position = new Vector2(0, (i + 1) * child.Size.Y + StartY);
                child.Layer = -10;
            }
        }
    }

    private void RedirectClicksToChildren()
    {
        if (Input.IsMouseButtonPressed(MouseButtonCode.Left) && OnTopLeft)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is Clickable child)
                {
                    child.OnTopLeft = true;
                }
            }

            OnTopLeft = false;
        }
    }

    private void HandleScrolling()
    {
        StartY += Input.GetMouseWheelMovement() * MovementSpeed;
    }
}