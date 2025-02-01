namespace Cherris;

public class TextureRectangle : Node2D
{
    public Texture? Texture { get; set; }

    static TextureRectangle()
    {
        PropertyRegistry.Register(typeof(TextureRectangle), new()
        {
            { "Texture", (node, value) => ((TextureRectangle)node).Texture = new(value.ToString()!) },
        });
    }

    public TextureRectangle()
    {
        Size = new(32, 32);
    }

    protected override void Draw()
    {
        if (Texture is null)
        {
            return;
        }

        DrawTextureScaled(
            Texture,
            GlobalPosition,
            Origin,
            0,
            Scale,
            false,
            false);

        Rectangle source = new(Vector2.Zero, Texture.Size);
        Rectangle target = new(GlobalPosition, FinalSize);

        DrawTexturedRectangle(
            Texture,
            source,
            target,
            Origin,
            Rotation
        );
    }
}