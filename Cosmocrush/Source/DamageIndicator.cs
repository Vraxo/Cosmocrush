using Cherris;
using Raylib_cs;

namespace Cosmocrush;

public class DamageIndicator : Label
{
    public int Health { get; set; } = 0;
    public int MaxHealth { get; set; } = 0;

    private const float speed = 100;
    private readonly Timer? destructionTimer;

    private float _animatedAlpha = 1.0f;
    public float AnimatedAlpha
    {
        get => _animatedAlpha;

        set
        {
            _animatedAlpha = float.Clamp(value, 0f, 1f);
            UpdateAlpha();
        }
    }

    // Main

    public override void Ready()
    {
        base.Ready();

        destructionTimer!.Timeout += OnTimerTimeout;
        SetOutlineColor();
    }

    public override void Process()
    {
        base.Process();

        Position = new(
            Position.X,
            Position.Y - speed * TimeServer.Delta);
    }

    // Other

    private void OnTimerTimeout(Timer timer)
    {
        Free();
    }

    private void UpdateAlpha()
    {
        Theme.FontColor = new(
            (byte)1,
            (byte)1,
            (byte)1,
            (byte)AnimatedAlpha);
    }

    private void SetOutlineColor()
    {
        if (MaxHealth <= 0)
        {
            Theme.OutlineColor = Color.White;
            return;
        }

        float ratio = float.Clamp((float)Health / MaxHealth, 0f, 1f);

        // Interpolate hue from 0° (red) to 120° (green)
        float hue = float.Lerp(0f, 120f, ratio);

        Color outlineColor = Raylib.ColorFromHSV(hue, 1f, 1f);

        Theme.OutlineColor = outlineColor;
    }
}