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

    public BodyType Type { get; set; } = BodyType.Dynamic;
    public bool FixedRotation { get; set; } = false;
    public BoxCollider Collider { get; set; }
    
    public Body? Box2DBody;

    public Vector2 Velocity
    {
        get => Box2DBody?.GetLinearVelocity() ?? Vector2.Zero;
        set => Box2DBody?.SetLinearVelocity(value);
    }

    public Vector2 LinearVelocity
    {
        get => Box2DBody?.GetLinearVelocity() ?? Vector2.Zero;
        set => Box2DBody?.SetLinearVelocity(value);
    }

    public float GravityScale
    {
        get => Box2DBody?.GetGravityScale() ?? field;
        set
        {
            field = value;
            Box2DBody?.SetGravityScale(value);
        }
    } = 1;

    public float LinearDamping
    {
        get => Box2DBody?.GetLinearDamping() ?? field;
        set => Box2DBody?.SetLinearDampling(value);
    }

    // Main

    public RigidBody()
    {
        Collider = (BoxCollider)AddChild(new BoxCollider(), "BoxCollider");
    }

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

    // Apply

    public void ApplyForce(Vector2 force)
    {
        Box2DBody?.ApplyForceToCenter(force);
    }

    public void ApplyLinearImpulse(Vector2 impulse, Vector2 point)
    {
        Box2DBody?.ApplyLinearImpulse(impulse, point);
    }

    public void ApplyForceToCenter(Vector2 force)
    {
        Box2DBody?.ApplyForceToCenter(force);
    }

    public void ApplyLinearImpulseToCenter(Vector2 impulse)
    {
        Box2DBody?.ApplyLinearImpulseToCenter(impulse);
    }
}