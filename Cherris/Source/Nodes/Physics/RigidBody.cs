using Box2D.NetStandard.Dynamics.Bodies;

namespace Cherris;

public class RigidBody : Node2D
{
    public enum BodyType
    {
        Static,
        Dynamic,
        Kinematic
    }

    // Properties
    public BodyType Type { get; set; } = BodyType.Dynamic;
    public bool FixedRotation { get; set; } = false;
    public BoxCollider Collider { get; set; }

    internal Body? Box2DBody;

    private bool enabled = true;
    private float gravityScale = 1;
    private float linearDamping;

    /// <summary>
    /// Whether the RigidBody is enabled and participating in the physics simulation.
    /// </summary>
    public bool Enabled
    {
        get => enabled;
        set
        {
            if (enabled == value) return;
            enabled = value;

            if (enabled)
            {
                PhysicsServer.Instance.CreateBox2DBody(this);
            }
            else
            {
                PhysicsServer.Instance.DestroyBox2DBody(this);
                Box2DBody = null;
            }
        }
    }

    /// <summary>
    /// The linear velocity of the RigidBody.
    /// </summary>
    public Vector2 Velocity
    {
        get => Box2DBody?.GetLinearVelocity() ?? Vector2.Zero;
        set => Box2DBody?.SetLinearVelocity(value);
    }

    /// <summary>
    /// The linear velocity of the RigidBody (alias for Velocity).
    /// </summary>
    public Vector2 LinearVelocity
    {
        get => Box2DBody?.GetLinearVelocity() ?? Vector2.Zero;
        set => Box2DBody?.SetLinearVelocity(value);
    }

    /// <summary>
    /// The gravity scale applied to the RigidBody.
    /// </summary>
    public float GravityScale
    {
        get => Box2DBody?.GetGravityScale() ?? gravityScale;
        set
        {
            gravityScale = value;
            Box2DBody?.SetGravityScale(value);
        }
    }

    /// <summary>
    /// The linear damping applied to the RigidBody.
    /// </summary>
    public float LinearDamping
    {
        get => Box2DBody?.GetLinearDamping() ?? linearDamping;
        set
        {
            linearDamping = value;
            Box2DBody?.SetLinearDampling(value);
        }
    }

    /// <summary>
    /// The mass of the RigidBody.
    /// </summary>
    public float Mass
    {
        get => Box2DBody?.GetMass() ?? field;
    }

    // Constructor
    public RigidBody()
    {
        Collider = (BoxCollider)AddChild(new BoxCollider(), "BoxCollider");
    }

    // Lifecycle Methods
    public override void Ready()
    {
        base.Ready();
        PhysicsServer.Instance.Register(this);
    }

    public override void Free()
    {
        base.Free();
        PhysicsServer.Instance.Unregister(this);
    }

    // Transform Methods
    public void SetTransform(Vector2 position, float angle)
    {
        Box2DBody?.SetTransform(position, angle);
    }

    // Force and Impulse Methods
    public void ApplyForce(Vector2 force)
    {
        Box2DBody?.ApplyForceToCenter(force);
    }

    public void ApplyForceToCenter(Vector2 force)
    {
        Box2DBody?.ApplyForceToCenter(force);
    }

    public void ApplyLinearImpulse(Vector2 impulse, Vector2 point)
    {
        Box2DBody?.ApplyLinearImpulse(impulse, point);
    }

    public void ApplyLinearImpulseToCenter(Vector2 impulse)
    {
        if (Box2DBody is null)
        {
            Log.Error("Box2D body is null.");
            return;
        }

        Box2DBody.ApplyLinearImpulseToCenter(impulse);
    }
}