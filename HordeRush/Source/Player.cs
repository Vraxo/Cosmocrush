using Nodica;

namespace Cosmocrush;

public class Player : ColliderRectangle
{
    public float Speed = 200f;

    public override void Ready()
    {
        base.Ready();
        _ = GetNode<Enemy>("/root/Enemy1");
    }

    public override void Update()
    {
        base.Update();

        Position += Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
    }
}