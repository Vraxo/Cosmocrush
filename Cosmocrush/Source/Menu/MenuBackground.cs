using Cherris;

namespace Cosmocrush;

public class MenuBackground : TextureRectangle
{
    private readonly string pathTemplate = "Res/Sprites/MenuBackgrounds/MenuBackground{0}.png";
    private float oscillationSpeed = 1.0f;
    private float amplitude = 20.0f;

    public override void Ready()
    {
        base.Ready();

        Random random = new();
        string path = string.Format(pathTemplate, random.Next(1, 7)); // Generates a number in the range [1, 6]
        Texture = ResourceLoader.Load<Texture>(path);
    }

    public override void Update()
    {
        base.Update();

        Size = WindowManager.Size * 1.5f;

        // Calculate the center position of the window
        Vector2 windowCenter = WindowManager.Size / 2;

        // Calculate oscillating offsets
        float offsetX = amplitude * MathF.Sin(TimeManager.Elapsed * oscillationSpeed);
        float offsetY = amplitude * MathF.Cos(TimeManager.Elapsed * oscillationSpeed * 0.8f); // Slightly slower Y movement

        // Update the position to stay centered with oscillation
        Position = windowCenter + new Vector2(offsetX, offsetY);
    }
}