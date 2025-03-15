#pragma warning disable 0649

using Cherris;

namespace Cosmocrush;

public class Menu : Node
{
    public override void Ready()
    {
        base.Ready();
        GameSettings.Instance.Load();
    }

    public override void Process()
    {
        base.Process();

        Raylib_cs.Raylib.DrawFPS(10, 10);

        GetNode<ParticleEmitter>("Particles").SpawnAreaMax = new(0, DisplayServer.WindowSize.Y);
    }
}