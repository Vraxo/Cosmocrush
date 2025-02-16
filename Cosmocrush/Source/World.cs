namespace Cherris;

public class World : Node
{
    public override void Process()
    {
        base.Process();

        if (Input.IsKeyPressed(KeyCode.Space))
        {
            Tree.Paused = !Tree.Paused;
        }
    }
}