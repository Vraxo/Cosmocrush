using System.Reflection;
using YamlDotNet.Serialization;
using Raylib_cs;
using System.Runtime.Loader;

namespace Cherris;

public sealed class AppServer
{
    public static AppServer Instance { get; } = new();

    private const string configFilePath = "Res/Cherris/Config.yaml";
    private const string logFilePath = "Res/Cherris/Log.txt";

    private AppServer()
    {
        AppServer.LoadConfig();
    }

    public void Run()
    {
        Start();
        Update();
    }

    private void Start()
    {
        CreateLogFile();
        SetCurrentDirectory();
        ApplyConfig();
    }

    private static void SetRootNodeFromConfig(string scenePath)
    {
        PackedScene packedScene = new(scenePath);
        SceneTree.Instance.RootNode = packedScene.Instantiate<Node>();
    }

    private static void Update()
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            ProcessServers();
            SceneTree.Instance.Process();
            Raylib.EndDrawing();
        }
    }

    private static void ProcessServers()
    {
        TimeServer.Instance.Process();
        ClickServer.Instance.Process();
        CollisionServer.Instance.Process();
        RenderServer.Instance.Process();
    }

    private static void CreateLogFile()
    {
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
            File.Create(logFilePath);
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

    private static Configuration LoadConfig()
    {
        var deserializer = new DeserializerBuilder().Build();
        string yaml = File.ReadAllText(configFilePath);
        return deserializer.Deserialize<Configuration>(yaml);
    }

    private void ApplyConfig()
    {
        Configuration config = LoadConfig() ?? throw new Exception("Config file is invalid.");

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
}