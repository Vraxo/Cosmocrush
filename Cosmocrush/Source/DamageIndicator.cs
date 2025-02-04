using System;
using Cherris;
using Raylib_cs;

namespace Cosmocrush;

public class DamageIndicator : Label
{
    public int Health { get; set; } = 0;
    public int MaxHealth { get; set; } = 0;

    private readonly float speed = 100;
    private Animation animation = ResourceLoader.Load<Animation>("Res/Animations/DamageIndicator.anim.yaml");

    private readonly Timer? destructionTimer;
    private readonly AnimationPlayer? animationPlayer;

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

    public override void Ready()
    {
        base.Ready();

        InheritScale = false;
        destructionTimer!.Timeout += OnTimerTimeout;
        Scale = new(2);

        animationPlayer!.Play(animation);

        SetOutlineColor();
    }

    public override void Update()
    {
        base.Update();

        Position = new(
            Position.X,
            Position.Y - speed * TimeServer.Delta);
    }

    private void UpdateAlpha()
    {
        Theme.FontColor = new(
            (byte)1,
            (byte)1,
            (byte)1,
            (byte)AnimatedAlpha);
    }

    private void OnTimerTimeout(Timer timer)
    {
        Free();
    }

    private void SetOutlineColor()
    {
        if (MaxHealth <= 0)
        {
            Theme.OutlineColor = Color.White;
            return;
        }

        float ratio = float.Clamp(Health / MaxHealth, 0f, 1f);

        var outlineColor = Raylib.ColorFromHSV(
            float.Lerp(0f, 0.333f, ratio),
            1f,
            1f
        );

        Theme.OutlineColor = outlineColor;
    }
}