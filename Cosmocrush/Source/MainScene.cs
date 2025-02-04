using Cherris;

namespace Cosmocrush;

public class MainScene : Node
{
    private ColorRectangle background = new();

    public override void Ready()
    {
        base.Ready();

        background = GetNode<ColorRectangle>("Background");

        //GetNode<NavigationObstacle>("UpperWall").Position = new(Window.WindowSize.X / 2, 0);
        ////GetNode<NavigationObstacle>("UpperWall").GetNode<ColliderRectangle>("Collider").WindowSize = new(Window.OriginalWindowSize.X, 80);
        //GetNode<NavigationObstacle>("UpperWall").GetNode<ColliderRectangle>("Collider").Scale = new(80, 1);
        //GetNode<NavigationObstacle>("UpperWall").Scale = new(100, 0.1f);
    }

    public override void Update()
    {
        base.Update();

        background.Position = VisualServer.WindowSize / 2;
    }
}