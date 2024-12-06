using System.Reflection;
using System.Text.Json;

namespace Nodica;

public class App
{
    private static App? _instance;
    public static App Instance => _instance ??= new();

    private readonly IGraphicsBackend backend;
    public Node? RootNode;

    private App()
    {
        backend = new Backends.RaylibBackend();
        Log.Info("[APP] Using Raylib as backend.");
    }

    public void Start()
    {
        Initialize();
        Run();
    }

    public void SetRootNode(Node node, bool packedScene = false)
    {
        RootNode = node;

        if (!packedScene)
        {
            RootNode.Make();
        }
    }

    private void Initialize()
    {
        SetCurrentDirectory();

        string configFilePath = "Resources/Nodica/Config.json";
        string configJson = File.ReadAllText(configFilePath);
        var config = JsonSerializer.Deserialize<Configuration>(configJson);

        if (config is null)
        {
            return;
        }

        Window.OriginalSize = new(config.Width, config.Height);

        backend.SetWindowFlags(config.ResizableWindow, config.AntiAliasing);

        backend.InitializeWindow(
            config.Width,
            config.Height,
            config.MinWidth,
            config.MinHeight,
            config.MaxWidth,
            config.MaxHeight,
            config.Title,
            "Resources/Icon/Icon.png");

        backend.SetWindowMinSize(config.Width, config.Height);

        SetRootNodeFromConfig(config.MainScenePath);
    }

    private void SetRootNodeFromConfig(string scenePath)
    {
        PackedScene packedScene = new(scenePath);
        RootNode = packedScene.Instantiate<Node>(true);
    }

    private void Run()
    {
        while (!backend.WindowShouldClose())
        {
            backend.BeginDrawing();
            backend.ClearBackground(DefaultTheme.Background);
            ProcessSingletons();
            RootNode?.Process();
            backend.EndDrawing();

            PrintTree();
        }
    }

    private static void ProcessSingletons()
    {
        ClickManager.Instance.Process();
        CollisionManager.Instance.Process();
    }

    private void PrintTree()
    {
        if (backend.IsKeyPressed(Key.Enter))
        {
            Console.Clear();
            RootNode?.PrintChildren();
        }
    }

    private static void SetCurrentDirectory()
    {
        string? assemblyLocation = Assembly.GetEntryAssembly()?.Location;
        string? directoryName = Path.GetDirectoryName(assemblyLocation);

        if (directoryName is null)
        {
            return;
        }

        Environment.CurrentDirectory = directoryName;
    }
}