using Cherris;

namespace Cosmocrush;

public class Menu : Node
{
    public override void Ready()
    {
        base.Ready();
        GameSettings.Instance.Load();
    }

    public override void Update()
    {
        base.Update();

        Raylib_cs.Raylib.DrawFPS(10, 10);

        GetNode<ParticleGenerator>("Particles").SpawnAreaMax = new(0, VisualServer.WindowSize.Y);
    }
}