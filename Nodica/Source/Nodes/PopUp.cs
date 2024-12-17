using System.Data;
using System.IO.Compression;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Nodica;

public class PopUp : ClickableRectangle
{
    public string Title { get; set; } = "";
    public float TitleBarHeight { get; set; } = 32;
    public Color TitleBarColor { get; set; } = DefaultTheme.Accent;
    public Alignment TitleAlignment = new();
    public Vector2 MinSize { get; set; } = new(640, 480);
    public Vector2 MaxSize { get; set; } = new(960, 720);

    private bool isDragging = false;
    private bool isResizingRight = false;
    private bool isResizingLeft = false;
    private bool isResizingBottom = false;
    private bool isResizingTop = false;
    private Vector2 dragOffset;

    public PopUp()
    {
        Size = new(640, 480);
        InheritPosition = false;
        Alignment.Vertical = VerticalAlignment.Top;
    }

    public override void Update()
    {
        base.Update();
        HandleResizing();
        HandleDragging();

        float width = Math.Clamp(Size.X, MinSize.X, MaxSize.X);
        float height = Math.Clamp(Size.Y, MinSize.Y, MaxSize.Y);
        Size = new(width, height);
    }

    protected override void Draw()
    {
        base.Draw();

        DrawBackground();
        DrawTitleBar();
        DrawTitle();
    }

    // Drawing

    private void DrawBackground()
    {
        DrawRoundedRectangle(
            GlobalPosition - Origin,
            Size,
            0,
            0,
            Color.DarkGray);
    }

    private void DrawTitleBar()
    {
        DrawRoundedRectangle(
            GlobalPosition - Origin,
            new(Size.X, TitleBarHeight),
            0,
            0,
            TitleBarColor);
    }

    private void DrawTitle()
    {
        DrawText(
            Title,
            GetTitlePosition(),
            FontManager.Instance.Get("RobotoMono 32"),
            16,
            0,
            Color.White);
    }

    private Vector2 GetTitlePosition()
    {
        Vector2 titleSize = Font.MeasureText(
            ResourceLoader.Load<Font>("RobotoMono 32"),
            Title,
            16);

        Vector2 topBarPosition = GlobalPosition - Origin;
        Vector2 topBarSize = new(Size.X, TitleBarHeight);

        float x = TitleAlignment.Horizontal switch
        {
            HorizontalAlignment.Center => topBarPosition.X + (topBarSize.X - titleSize.X) / 2,
            HorizontalAlignment.Left => topBarPosition.X + 8,
            HorizontalAlignment.Right => topBarPosition.X + topBarSize.X - titleSize.X - 8,
            _ => topBarPosition.X
        };

        float y = TitleAlignment.Vertical switch
        {
            VerticalAlignment.Center => topBarPosition.Y + (topBarSize.Y - titleSize.Y) / 2,
            VerticalAlignment.Top => topBarPosition.Y + 4,
            VerticalAlignment.Bottom => topBarPosition.Y + topBarSize.Y - titleSize.Y - 4,
            _ => topBarPosition.Y
        };

        return new(x, y);
    }

    // Dragging & resizing

    private void HandleDragging()
    {
        Vector2 mousePosition = Input.MousePosition;

        if (Input.IsMouseButtonDown(MouseButtonCode.Left))
        {
            if (!isDragging && !IsResizing() && IsMouseOnTitleBar())
            {
                isDragging = true;
                dragOffset = mousePosition - GlobalPosition;
            }

            if (isDragging)
            {
                GlobalPosition = mousePosition - dragOffset;
            }
        }
        else
        {
            isDragging = false;
        }
    }

    private void HandleResizing()
    {
        Vector2 mousePosition = Input.MousePosition;

        if (Input.IsMouseButtonDown(MouseButtonCode.Left))
        {
            if (!isResizingRight && IsMouseOnRightEdge() && !isResizingLeft)
            {
                isResizingRight = true;
                Alignment.Horizontal = HorizontalAlignment.Left;
                GlobalPosition = new(GlobalPosition.X - Size.X / 2, GlobalPosition.Y);
                MoveChildrenToLeft();
            }

            if (!isResizingLeft && IsMouseOnLeftEdge() && !isResizingRight)
            {
                isResizingLeft = true;
                Alignment.Horizontal = HorizontalAlignment.Right;
                GlobalPosition = new(GlobalPosition.X + Size.X / 2, GlobalPosition.Y);
                MoveChildrenToRight();
            }

            if (!isResizingBottom && IsMouseOnBottomEdge() && !isResizingTop)
            {
                isResizingBottom = true;
            }

            if (!isResizingTop && IsMouseOnTopEdge() && !isResizingBottom)
            {
                isResizingTop = true;
                Alignment.Vertical = VerticalAlignment.Bottom;
                GlobalPosition = new(GlobalPosition.X, GlobalPosition.Y + Size.Y);
                MoveChildrenDown();
            }

            if (isResizingRight)
            {
                DrawCircle(GlobalPosition - Origin, 1, Color.Blue);

                float difference = mousePosition.X - (GlobalPosition.X + Size.X);

                float previousX = Size.X;

                float width = Math.Clamp(Size.X + difference, MinSize.X, MaxSize.X);
                Size = new(width, Size.Y);

                foreach (Node node in Children)
                {
                    if (node is Node2D child)
                    {
                        child.Position = new(child.Position.X + (Size.X - previousX) / 2, child.Position.Y);
                    }
                }
            }

            if (isResizingLeft)
            {
                float difference = GlobalPosition.X - Size.X - mousePosition.X;

                float previousX = Size.X;

                float width = Math.Clamp(Size.X + difference, MinSize.X, MaxSize.X);
                Size = new(width, Size.Y);

                foreach (Node node in Children)
                {
                    if (node is Node2D child)
                    {
                        child.Position = new(child.Position.X - (Size.X - previousX) / 2, child.Position.Y);
                    }
                }
            }

            if (isResizingBottom)
            {
                float newHeight = mousePosition.Y - (GlobalPosition.Y - Origin.Y) - Size.Y;
                Size = new(Size.X, MathF.Max(Size.Y + newHeight, MinSize.Y));
            }

            if (isResizingTop)
            {
                float difference = mousePosition.Y - (GlobalPosition.Y - Origin.Y);

                float previousY = Size.Y;

                float height = Math.Clamp(Size.Y - difference, MinSize.Y, MaxSize.Y);
                Size = new(Size.X, height);

                foreach (Node node in Children)
                {
                    if (node is Node2D child)
                    {
                        child.Position = new(child.Position.X, child.Position.Y - (Size.Y - previousY));
                    }
                }
            }
        }
        else
        {
            isResizingRight = false;
            isResizingLeft = false;
            isResizingBottom = false;
            isResizingTop = false;

            if (Alignment.Horizontal == HorizontalAlignment.Left)
            {
                Alignment.Horizontal = HorizontalAlignment.Center;
                GlobalPosition = new(GlobalPosition.X + Size.X / 2, GlobalPosition.Y);
                MoveChildrenToRight();
            }
            else if (Alignment.Horizontal == HorizontalAlignment.Right)
            {
                Alignment.Horizontal = HorizontalAlignment.Center;
                GlobalPosition = new(GlobalPosition.X - Size.X / 2, GlobalPosition.Y);
                MoveChildrenToLeft();
            }
            else if (Alignment.Vertical == VerticalAlignment.Bottom)
            {
                Alignment.Vertical = VerticalAlignment.Top;
                GlobalPosition = new(GlobalPosition.X, GlobalPosition.Y - Size.Y);
                MoveChildrenUp();
            }
        }
    }

    // Checks

    private bool IsMouseOnRightEdge()
    {
        float edgeWidth = 8;
        Vector2 mousePosition = Input.MousePosition;
        Vector2 rightEdgePosition = GlobalPosition - Origin + new Vector2(Size.X - edgeWidth / 2, 0);

        return mousePosition.X >= rightEdgePosition.X &&
               mousePosition.X <= rightEdgePosition.X + edgeWidth &&
               mousePosition.Y >= rightEdgePosition.Y + TitleBarHeight &&
               mousePosition.Y <= rightEdgePosition.Y + Size.Y;
    }

    private bool IsMouseOnLeftEdge()
    {
        float edgeWidth = 8;
        Vector2 mousePosition = Input.MousePosition;
        Vector2 leftEdgePosition = GlobalPosition - Origin + new Vector2(-edgeWidth / 2, 0);

        return mousePosition.X >= leftEdgePosition.X &&
               mousePosition.X <= leftEdgePosition.X + edgeWidth &&
               mousePosition.Y >= leftEdgePosition.Y + TitleBarHeight &&
               mousePosition.Y <= leftEdgePosition.Y + Size.Y;
    }

    private bool IsMouseOnBottomEdge()
    {
        float edgeHeight = 8;
        Vector2 mousePosition = Input.MousePosition;
        Vector2 bottomEdgePosition = GlobalPosition - Origin + new Vector2(0, Size.Y - edgeHeight / 2);

        DrawCircle(GlobalPosition - Origin + new Vector2(0, Size.Y - 4), 1, Color.Red);

        return mousePosition.X >= bottomEdgePosition.X &&
               mousePosition.X <= bottomEdgePosition.X + Size.X &&
               mousePosition.Y >= bottomEdgePosition.Y &&
               mousePosition.Y <= bottomEdgePosition.Y + edgeHeight;
    }
    private bool IsMouseOnTopEdge()
    {
        float edgeHeight = TitleBarHeight / 3; // Top third
        Vector2 mousePosition = Input.MousePosition;
        Vector2 titleBarPosition = GlobalPosition - Origin; // Top-left corner of the title bar

        // Check if the mouse is within the top third of the title bar
        return mousePosition.X >= titleBarPosition.X &&
               mousePosition.X <= titleBarPosition.X + Size.X &&
               mousePosition.Y >= titleBarPosition.Y &&
               mousePosition.Y <= titleBarPosition.Y + edgeHeight;
    }

    private bool IsMouseOnTitleBar()
    {
        Vector2 mousePosition = Input.MousePosition;
        Vector2 titleBarPosition = GlobalPosition - Origin; // Top-left corner of the title bar
        float topEdgeHeight = TitleBarHeight / 3;          // Top third
        float bottomAreaHeight = TitleBarHeight - topEdgeHeight; // Bottom two-thirds

        // Check if the mouse is within the bottom two-thirds of the title bar
        return mousePosition.X >= titleBarPosition.X &&
               mousePosition.X <= titleBarPosition.X + Size.X &&
               mousePosition.Y >= titleBarPosition.Y + topEdgeHeight &&
               mousePosition.Y <= titleBarPosition.Y + TitleBarHeight;
    }

    private bool IsResizing()
    {
        return isResizingRight || isResizingBottom || isResizingLeft || isResizingTop;
    }

    // Move children

    private void MoveChildrenToRight()
    {
        foreach (Node node in Children)
        {
            if (node is Node2D child)
            {
                child.Position = new(child.Position.X - Size.X / 2, child.Position.Y);
            }
        }
    }

    private void MoveChildrenToLeft()
    {
        foreach (Node node in Children)
        {
            if (node is Node2D child)
            {
                child.Position = new(child.Position.X + Size.X / 2, child.Position.Y);
            }
        }
    }

    private void MoveChildrenDown()
    {
        foreach (Node node in Children)
        {
            if (node is Node2D child)
            {
                child.Position = new(child.Position.X, child.Position.Y - Size.Y);
            }
        }
    }

    private void MoveChildrenUp()
    {
        foreach (Node node in Children)
        {
            if (node is Node2D child)
            {
                child.Position = new(child.Position.X, child.Position.Y + Size.Y);
            }
        }
    }
}