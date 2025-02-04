using System.Reflection;
using YamlDotNet.Serialization;
using Raylib_cs;

namespace Cherris;

public sealed class AppServer
{
    private static AppServer? _instance;
    public static AppServer Instance => _instance ??= new();

    public Node? RootNode;

    private readonly Configuration config;

    private AppServer()
    {
        string configFilePath = "Res/Cherris/Config.yaml";
        config = LoadConfig(configFilePath);

        SceneConverter.ConvertYamlToBinary("Res/Scenes/Menu/Menu.yaml", "Res/Scenes/Menu/Menu.bin");
    }

    public void Run()
    {
        Start();
        Update();
    }

    public void SetRootNode(Node node, bool packedScene = false)
    {
        RootNode = node;

        if (!packedScene)
        {
            RootNode.Make();
        }
    }

    private void Start()
    {
        SetCurrentDirectory();

        if (config is null)
        {
            return;
        }

        VisualServer.OriginalWindowSize = new(config.Width, config.Height);

        ConfigFlags flags = ConfigFlags.VSyncHint | ConfigFlags.HighDpiWindow | ConfigFlags.AlwaysRunWindow;

        if (config.ResizableWindow)
        {
            flags |= ConfigFlags.ResizableWindow;
        }

        if (config.AntiAliasing)
        {
            flags |= ConfigFlags.Msaa4xHint;
        }

        Raylib.SetConfigFlags(flags);
        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.HighDpiWindow | ConfigFlags.AlwaysRunWindow);
        Raylib.InitWindow(config.Width, config.Height, config.Title);
        Raylib.SetWindowMinSize(config.MinWidth, config.MinHeight);
        Raylib.SetWindowMaxSize(config.MaxWidth, config.MaxHeight);
        Raylib.SetWindowIcon(Raylib.LoadImage("Res/Icon/Icon.png"));
        Raylib.InitAudioDevice();

        Raylib.SetWindowMinSize(config.MinWidth, config.MinHeight);

        SetRootNodeFromConfig(config.MainScenePath);
    }

    private void SetRootNodeFromConfig(string scenePath)
    {
        PackedScene packedScene = new(scenePath);
        RootNode = packedScene.Instantiate<Node>();
    }

    private void Update()
    {
        while (!Raylib.WindowShouldClose())
        {
            //Backend.Drawing.BeginDrawing();
            //Backend.Drawing.ClearBackground(DefaultTheme.Background);
            //RootNode?.Process();
            ProcessManagers();
            //Backend.Drawing.EndDrawing();

            PrintTree();
        }
    }

    private static void ProcessManagers()
    {
        TimeServer.Instance.Process();
        ClickServer.Instance.Process();
        CollisionServer.Instance.Process();
        RenderServer.Instance.Process();
    }

    private void PrintTree()
    {
        if (Input.IsKeyPressed(KeyCode.Enter))
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
        var deserializer = new DeserializerBuilder().Build();
        return deserializer.Deserialize<Configuration>(File.ReadAllText(filePath));
    }
}