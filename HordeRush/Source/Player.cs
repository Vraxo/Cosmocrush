using Nodica;

namespace HordeRush;

public class Player : ColliderRectangle
{
    public float Speed = 200f;

    public override void Update()
    {
        base.Update();

        Position += Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
    }
}