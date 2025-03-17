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

    internal Body? Box2DBody;

    public bool Enabled
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            if (field)
            {
                PhysicsServer.Instance.CreateBox2DBody(this);
            }
            else
            {
                PhysicsServer.Instance.DestroyBox2DBody(this);
                Box2DBody = null;
            }
        }
    } = true;

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
        set
        {
            field = value;
            Box2DBody?.SetLinearDampling(value);
        }
    }

    public float Mass
    {
        get => Box2DBody?.GetMass() ?? field;
    }

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