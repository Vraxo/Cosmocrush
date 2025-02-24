namespace Cherris;

public class PopUp : ClickableRectangle
{
    public string Title { get; set; } = "";
    public float TitleBarHeight { get; set; } = 32;
    public Color TitleBarColor { get; set; } = DefaultTheme.Accent;
    public Alignment TitleAlignment { get; set; } = new();
    public Vector2 MinSize { get; set; } = new(640, 480);
    public Vector2 MaxSize { get; set; } = new(960, 720);
    public bool ClipChildren { get; set; } = false;
    public BoxStyle TitleBarTheme { get; set; } = new();

    private const float EdgeWidth = 8f;
    private const float EdgeHeight = 8f;

    private bool dragging = false;
    private bool resizingRight = false;
    private bool resizingLeft = false;
    private bool resizingBottom = false;
    private bool resizingTop = false;
    private Vector2 dragOffset;

    // Main

    public PopUp()
    {
        Size = new(640, 480);
        InheritPosition = false;
        VAlignment = VAlignment.Top;
        TitleBarTheme.Roundness = 0;
    }

    public override void Process()
    {
        if (ClipChildren)
        {
            RenderServer.BeginScissorMode(GlobalPosition - Origin, Size);
        }

        base.Process();

        UpdateResizingCursor();
        HandleResizing();
        HandleDragging();

        if (ClipChildren)
        {
            RenderServer.EndScissorMode();
        }
    }

    public override void Draw()
    {
        base.Draw();

        DrawBackground();
        DrawTitleBar();
        DrawTitle();
    }

    // Drawing

    private void DrawBackground()
    {
        DrawRectangleRounded(
            GlobalPosition - Origin,
            Size,
            0,
            0,
            Color.White);
    }

    private void DrawTitleBar()
    {
        DrawRectangleThemed(
            GlobalPosition - Origin,
            new(Size.X, TitleBarHeight),
            TitleBarTheme);
    }

    private void DrawTitle()
    {
        DrawText(
            Title,
            GetTitlePosition(),
            ResourceLoader.Load<Font>("Res/Cherris/Fonts/RobotoMono.ttf:32"),
            16,
            0,
            Color.White);
    }

    private Vector2 GetTitlePosition()
    {
        Vector2 titleSize = Font.MeasureText(
            ResourceLoader.Load<Font>("Res/Cherris/Fonts/RobotoMono.ttf:32"),
            Title,
            16,
            0);

        Vector2 topBarPosition = GlobalPosition - Origin;
        Vector2 topBarSize = new(Size.X, TitleBarHeight);

        float x = TitleAlignment.Horizontal switch
        {
            HAlignment.Center => topBarPosition.X + (topBarSize.X - titleSize.X) / 2,
            HAlignment.Left => topBarPosition.X + 8,
            HAlignment.Right => topBarPosition.X + topBarSize.X - titleSize.X - 8,
            _ => topBarPosition.X
        };

        float y = TitleAlignment.Vertical switch
        {
            VAlignment.Center => topBarPosition.Y + (topBarSize.Y - titleSize.Y) / 2,
            VAlignment.Top => topBarPosition.Y + 4,
            VAlignment.Bottom => topBarPosition.Y + topBarSize.Y - titleSize.Y - 4,
            _ => topBarPosition.Y
        };

        return new(x, y);
    }

    // Dragging

    private void HandleDragging()
    {
        Vector2 mousePosition = Input.MousePosition;
        bool isMouseDown = Input.IsMouseButtonDown(MouseButtonCode.Left);

        if (isMouseDown)
        {
            bool canStartDragging = !dragging && !IsResizing() && IsMouseOnTitleBar();

            if (canStartDragging)
            {
                dragging = true;
                dragOffset = mousePosition - GlobalPosition;
            }

            bool shouldUpdatePosition = dragging && IsMouseInWindow(mousePosition);

            if (shouldUpdatePosition)
            {
                GlobalPosition = mousePosition - dragOffset;
            }
        }
        else
        {
            dragging = false;
        }
    }

    // Resizing

    private void UpdateResizingCursor()
    {
        bool right;
        bool left;
        bool top;
        bool bottom;

        bool anyResizing = resizingRight || resizingLeft || resizingTop || resizingBottom;

        if (anyResizing)
        {
            right = resizingRight;
            left = resizingLeft;
            top = resizingTop;
            bottom = resizingBottom;
        }
        else
        {
            right = IsMouseOnEdge(Direction.Right);
            left = IsMouseOnEdge(Direction.Left);
            top = IsMouseOnEdge(Direction.Up);
            bottom = IsMouseOnEdge(Direction.Down);
        }

        if ((right && bottom) || (left && top))
        {
            Input.Cursor = MouseCursorCode.ResizeBottomLeftToTopRight;
        }
        else if ((right && top) || (left && bottom))
        {
            Input.Cursor = MouseCursorCode.ResizeTopLeftToBottomRight;
        }
        else if (right || left)
        {
            Input.Cursor = MouseCursorCode.ResizeHorizontal;
        }
        else if (top || bottom)
        {
            Input.Cursor = MouseCursorCode.ResizeVertical;
        }
        else
        {
            Input.Cursor = MouseCursorCode.Default;
        }
    }

    private void HandleResizing()
    {
        if (!Input.IsMouseButtonDown(MouseButtonCode.Left) || dragging)
        {
            ResetAfterResize();
            return;
        }

        CheckForResizeStart();

        Vector2 mousePosition = Input.MousePosition;

        if (resizingRight)
        {
            ResizeHorizontal(mousePosition, true);
        }
        
        if (resizingLeft)
        {
            ResizeHorizontal(mousePosition, false);
        }

        if (resizingBottom)
        {
            ResizeDown(mousePosition);
        }

        if (resizingTop)
        {
            ResizeUp(mousePosition);
        }
    }

    private void ResetAfterResize()
    {
        resizingRight = false;
        resizingLeft = false;
        resizingBottom = false;
        resizingTop = false;

        if (HAlignment == HAlignment.Left)
        {
            HAlignment = HAlignment.Center;
            GlobalPosition = new(GlobalPosition.X + Size.X / 2, GlobalPosition.Y);
            MoveChildrenTo(Direction.Right);
        }
        else if (HAlignment == HAlignment.Right)
        {
            HAlignment = HAlignment.Center;
            GlobalPosition = new(GlobalPosition.X - Size.X / 2, GlobalPosition.Y);
            MoveChildrenTo(Direction.Left);
        }
        else if (VAlignment == VAlignment.Bottom)
        {
            VAlignment = VAlignment.Top;
            GlobalPosition = new(GlobalPosition.X, GlobalPosition.Y - Size.Y);
            MoveChildrenTo(Direction.Up);
        }
    }

    private void CheckForResizeStart()
    {
        if (!resizingRight && IsMouseOnEdge(Direction.Right) && !resizingLeft)
        {
            resizingRight = true;
            HAlignment = HAlignment.Left;
            GlobalPosition = new(GlobalPosition.X - Size.X / 2, GlobalPosition.Y);
            MoveChildrenTo(Direction.Left);
        }

        if (!resizingLeft && IsMouseOnEdge(Direction.Left) && !resizingRight)
        {
            resizingLeft = true;
            HAlignment = HAlignment.Right;
            GlobalPosition = new(GlobalPosition.X + Size.X / 2, GlobalPosition.Y);
            MoveChildrenTo(Direction.Right);
        }

        if (!resizingBottom && IsMouseOnEdge(Direction.Down) && !resizingTop)
        {
            resizingBottom = true;
        }

        if (!resizingTop && IsMouseOnEdge(Direction.Up) && !resizingBottom)
        {
            resizingTop = true;
            VAlignment = VAlignment.Bottom;
            GlobalPosition = new(GlobalPosition.X, GlobalPosition.Y + Size.Y);
            MoveChildrenTo(Direction.Down);
        }
    }

    private void AdjustChildrenPositions(float deltaX, float deltaY)
    {
        foreach (Node node in Children)
        {
            if (node is not Node2D child)
            {
                continue;
            }

            child.Position += new Vector2(deltaX, deltaY);
        }
    }

    // Resize

    private void ResizeHorizontal(Vector2 mousePosition, bool isRightEdge)
    {
        float previousWidth = Size.X;
        float difference = isRightEdge
            ? mousePosition.X - (GlobalPosition.X + previousWidth)
            : (GlobalPosition.X - previousWidth) - mousePosition.X;

        if (mousePosition.X < 0 || mousePosition.X > VisualServer.WindowSize.X)
        {
            return;
        }

        var newWidth = float.Clamp(previousWidth + difference, MinSize.X, MaxSize.X);
        Size = new Vector2(newWidth, Size.Y);

        float deltaX = (newWidth - previousWidth) / 2 * (isRightEdge ? 1 : -1);
        AdjustChildrenPositions(deltaX, 0);
    }

    private void ResizeUp(Vector2 mousePosition)
    {
        float previousY = Size.Y;
        float difference = mousePosition.Y - (GlobalPosition.Y - Origin.Y);

        if (mousePosition.Y < 0 || mousePosition.Y > VisualServer.WindowSize.Y)
        {
            return;
        }

        var newHeight = float.Clamp(Size.Y - difference, MinSize.Y, MaxSize.Y);
        Size = new Vector2(Size.X, newHeight);
        AdjustChildrenPositions(0, -(newHeight - previousY));
    }

    private void ResizeDown(Vector2 mousePosition)
    {
        if (mousePosition.Y < 0 || mousePosition.Y > VisualServer.WindowSize.Y)
        {
            return;
        }

        float heightDelta = mousePosition.Y - (GlobalPosition.Y - Origin.Y + Size.Y);
        var newHeight = float.Max(Size.Y + heightDelta, MinSize.Y);
        Size = new(Size.X, newHeight);
    }

    // Checks

    private static bool IsMouseInArea(Vector2 position, Vector2 size)
    {
        Vector2 mouse = Input.MousePosition;

        return mouse.X >= position.X &&
               mouse.X <= position.X + size.X &&
               mouse.Y >= position.Y &&
               mouse.Y <= position.Y + size.Y;
    }

    private bool IsMouseOnEdge(Direction edge)
    {
        Vector2 basePosition = GlobalPosition - Origin;

        (Vector2 position, Vector2 size) = edge switch
        {
            Direction.Right => (
                basePosition + new Vector2(Size.X - EdgeWidth / 2, 0),
                new(EdgeWidth, Size.Y)
            ),
            Direction.Left => (
                basePosition - new Vector2(EdgeWidth / 2, 0),
                new(EdgeWidth, Size.Y)
            ),
            Direction.Down => (
                basePosition + new Vector2(0, Size.Y - EdgeHeight / 2),
                new(Size.X, EdgeHeight)
            ),
            Direction.Up => (
                basePosition,
                new(Size.X, TitleBarHeight / 8)
            ),
            _ => (Vector2.Zero, Vector2.Zero)
        };

        return IsMouseInArea(position, size);
    }

    private bool IsMouseOnTitleBar()
    {
        float topEdgeHeight = TitleBarHeight / 8;
        Vector2 areaPos = GlobalPosition - Origin + new Vector2(0, topEdgeHeight);
        Vector2 areaSize = new(Size.X, TitleBarHeight - topEdgeHeight);
        return IsMouseInArea(areaPos, areaSize);
    }

    private static bool IsMouseInWindow(Vector2 position)
    {
        return position.X >= 0 &&
               position.X <= VisualServer.WindowSize.X &&
               position.Y >= 0 &&
               position.Y <= VisualServer.WindowSize.Y;
    }

    private bool IsResizing()
    {
        return resizingRight || resizingBottom || resizingLeft || resizingTop;
    }

    // Move children

    private void MoveChildrenTo(Direction direction)
    {
        foreach (Node node in Children)
        {
            if (node is not Node2D child)
            {
                continue;
            }

            child.Position = direction switch
            {
                Direction.Left => new(child.Position.X + Size.X / 2, child.Position.Y),
                Direction.Up => new(child.Position.X, child.Position.Y + Size.Y),
                Direction.Right => new(child.Position.X - Size.X / 2, child.Position.Y),
                Direction.Down => new(child.Position.X, child.Position.Y - Size.Y),
                _ => new(child.Position.X, child.Position.Y)
            };
        }
    }

    private enum Direction { Left, Up, Right, Down }
}