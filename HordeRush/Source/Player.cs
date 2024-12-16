using Nodica;

namespace HordeRush;

public class Player : ColliderRectangle
{
    public float Speed = 200f;

    private Sprite sprite = new();

    public override void Ready()
    {
        base.Ready();

        sprite = GetNode<Sprite>("Sprite");
    }

    public override void Update()
    {
        base.Update();

        sprite.FlipH = Input.MousePosition.X <= GlobalPosition.X;
        Position += Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
    }
}