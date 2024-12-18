namespace Nodica;

public abstract class ClickableRectangle : Clickable
{
    public override bool IsMouseOver()
    {
        Vector2 mousePosition = Input.MousePosition;

        Vector2 position = GlobalPosition;
        Vector2 scaledOrigin = Scale * Origin;
        Vector2 scaledSize = Scale * Size;

        bool isMouseOver = 
            mousePosition.X > position.X - scaledOrigin.X &&
            mousePosition.X < position.X + scaledSize.X - scaledOrigin.X &&
            mousePosition.Y > position.Y - scaledOrigin.Y &&
            mousePosition.Y < position.Y + scaledSize.Y - scaledOrigin.Y;

        return isMouseOver;
    }
}