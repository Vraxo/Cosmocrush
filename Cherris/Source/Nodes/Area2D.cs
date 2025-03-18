using Box2D.NetStandard.Dynamics.Bodies;

namespace Cherris;

public class Area2D : Node2D
{
    public event Action<Node>? BodyEntered;
    public event Action<Node>? BodyExited;

    public HashSet<Node> OverlappingBodies { get; } = [];

    public BoxCollider Collider { get; set; } = new BoxCollider();

    public Body? Box2DBody { get; set; }

    public Vector2 LinearVelocity
    {
        get => Box2DBody?.GetLinearVelocity() ?? Vector2.Zero;
        set => Box2DBody?.SetLinearVelocity(value);
    }

    public bool Enabled 
    { 
        get;

        set
        {
            if (field == value)
            {
                return;
            }
            
            field = value;

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
    } = true;

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
}