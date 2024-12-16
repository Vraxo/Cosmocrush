using System.Reflection;
using YamlDotNet.Serialization;
using Nodica.Backends;

namespace Nodica;

public sealed class App
{
    private static App? _instance;
    public static App Instance => _instance ??= new();

    public readonly Backend Backend;
    public Node? RootNode;

    private App()
    {
        string configFilePath = "Res/Nodica/Config.yaml";
        Configuration config = LoadConfig(configFilePath);

        if (config?.Backend == "Raylib")
        {
            Backend = new RaylibBackend();
            Log.Info("[APP] Using Raylib as backend.");
        }
        else if (config?.Backend == "SDL2")
        {
            Backend = new SDL2Backend();
            Log.Info("[APP] Using SDL2 as backend.");
        }
        else
        {
            throw new InvalidOperationException("Invalid backend specified in the configuration.");
        }
    }

    public void Run()
    {
        Initialize();
        Loop();
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

        string configFilePath = "Res/Nodica/Config.yaml";
        Configuration config = LoadConfig(configFilePath);

        if (config is null)
        {
            return;
        }

        Window.OriginalSize = new(config.Width, config.Height);

        Backend.Window.SetWindowFlags(config.ResizableWindow, config.AntiAliasing);

        Backend.Window.InitializeWindow(
            config.Width,
            config.Height,
            config.MinWidth,
            config.MinHeight,
            config.MaxWidth,
            config.MaxHeight,
            config.Title,
            "Res/Icon/Icon.png");

        Backend.Window.SetWindowMinSize(config.Width, config.Height);

        SetRootNodeFromConfig(config.MainScenePath);
    }

    private void SetRootNodeFromConfig(string scenePath)
    {
        PackedScene2 packedScene = new(scenePath);
        RootNode = packedScene.Instantiate<Node>(true);
    }

    private void Loop()
    {
        while (!Backend.Window.WindowShouldClose())
        {
            Backend.Drawing.BeginDrawing();
            Backend.Drawing.ClearBackground(DefaultTheme.Background);
            ProcessManagers();
            RootNode?.Process();
            Backend.Drawing.EndDrawing();

            PrintTree();
        }
    }

    private static void ProcessManagers()
    {
        Time.Process();
        ClickManager.Instance.Process();
        CollisionManager.Instance.Process();
    }

    private void PrintTree()
    {
        if (Backend.Input.IsKeyPressed(KeyCode.Enter))
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

    private static Configuration LoadConfig(string filePath)
    {
        var yaml = new DeserializerBuilder().Build();
        using StreamReader reader = new(filePath);
        return yaml.Deserialize<Configuration>(reader);
    }
}