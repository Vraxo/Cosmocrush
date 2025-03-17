using Box2D.NetStandard.Dynamics.Bodies;

namespace Cherris;

public class Area2D : Node2D
{
    public event Action<Node>? BodyEntered;
    public event Action<Node>? BodyExited;

    public HashSet<Node> OverlappingBodies { get; } = new();

    public BoxCollider Collider { get; set; } = new BoxCollider();

    public Body? Box2DBody { get; set; }

    public bool Enabled { get; set; } = true;

    public Area2D()
    {
        Collider.IsSensor = true;
    }

    public override void Ready()
    {
        base.Ready();
        if (Enabled)
        {
            PhysicsServer.Instance.CreateBox2DAreaBody(this);
        }
    }
    public override void Free()
    {
        base.Free(); // Calls parent cleanup

        PhysicsServer.Instance.QueueFreeNode(this);
    }

    public override void Process()
    {
        base.Process();
        if (Enabled && Box2DBody != null)
        {
            PhysicsServer.Instance.SyncAreaBodyTransform(this);
        }
    }

    internal void OnBodyEnteredInternal(Node node)
    {
        OverlappingBodies.Add(node);
        BodyEntered?.Invoke(node);
    }

    internal void OnBodyExitedInternal(Node node)
    {
        OverlappingBodies.Remove(node);
        BodyExited?.Invoke(node);
    }

    // Correctly override the property setters, not methods
    public override Vector2 GlobalPosition
    {
        get => base.GlobalPosition;
        set
        {
            base.GlobalPosition = value; // Call base setter first
            if (Box2DBody != null && Enabled)
            {
                PhysicsServer.Instance.SyncAreaBodyTransform(this);
            }
        }
    }

    public override float Rotation
    {
        get => base.Rotation;
        set
        {
            base.Rotation = value; // Call base setter first
            if (Box2DBody != null && Enabled)
            {
                PhysicsServer.Instance.SyncAreaBodyTransform(this);
            }
        }
    }

    public bool IsEnabled() => Enabled;
    public void SetEnabled(bool enabled)
    {
        if (Enabled == enabled) return;
        Enabled = enabled;
        if (Enabled)
        {
            PhysicsServer.Instance.CreateBox2DAreaBody(this);
        }
        else
        {
            PhysicsServer.Instance.DestroyBox2DAreaBody(this);
            Box2DBody = null;
        }
    }
}