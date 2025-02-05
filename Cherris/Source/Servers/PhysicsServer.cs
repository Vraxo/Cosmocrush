namespace Cherris;

public sealed class PhysicsServer
{
    private static PhysicsServer? _instance;
    public static PhysicsServer Instance => _instance ??= new();

    public List<KinematicBody> KinematicBodies { get; } = [];

    public void RegisterKinematicBody(KinematicBody body)
    {
        if (!KinematicBodies.Contains(body))
        {
            KinematicBodies.Add(body);
        }
    }
}