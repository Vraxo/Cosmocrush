namespace Nodica;

public class TextureRectangle : Node2D
{
    public Texture? Texture { get; set; }

    public TextureRectangle()
    {
        Size = new(32, 32);
    }

    protected override void Draw()
    {
        DrawTexture(
            Texture,
            GlobalPosition - Origin,
            0,
            Scale,
            Color.White);
    }
}