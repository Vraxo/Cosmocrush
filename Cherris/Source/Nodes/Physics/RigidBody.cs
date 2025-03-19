using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using System.Numerics;

namespace Cherris;

public class RigidBody : CollisionObject2D
{
    public enum BodyType
    {
        Static,
        Dynamic,
        Kinematic
    }

    public BodyType Type { get; set; } = BodyType.Dynamic;
    public bool FixedRotation { get; set; } = false;

    internal Body? Box2DBody;

    private bool _enabled = true;
    private float _gravityScale = 1;
    private float _linearDamping;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value) return;

            _enabled = value;

            if (_enabled)
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
        get => Box2DBody?.GetGravityScale() ?? _gravityScale;
        set
        {
            _gravityScale = value;
            Box2DBody?.SetGravityScale(value);
        }
    }

    public float LinearDamping
    {
        get => Box2DBody?.GetLinearDamping() ?? _linearDamping;
        set
        {
            _linearDamping = value;
            Box2DBody?.SetLinearDampling(value);
        }
    }

    public float Mass => Box2DBody?.GetMass() ?? 0;

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

    public void ApplyTorque(float torque)
    {
        Box2DBody?.ApplyTorque(torque);
    }

    public void ApplyAngularImpulse(float impulse)
    {
        Box2DBody?.ApplyAngularImpulse(impulse);
    }

    public Vector2 GetCenterOfMass()
    {
        return Box2DBody?.GetWorldCenter() ?? Vector2.Zero;
    }

    public float GetInertia()
    {
        return Box2DBody?.GetInertia() ?? 0;
    }

    public void SetSleepingAllowed(bool allowed)
    {
        Box2DBody?.SetSleepingAllowed(allowed);
    }

    public bool IsSleepingAllowed()
    {
        return Box2DBody?.IsSleepingAllowed() ?? false;
    }

    public void SetAwake(bool awake)
    {
        Box2DBody?.SetAwake(awake);
    }

    public bool IsAwake()
    {
        return Box2DBody?.IsAwake() ?? false;
    }

    public void SetBullet(bool bullet)
    {
        Box2DBody?.SetBullet(bullet);
    }

    public bool IsBullet()
    {
        return Box2DBody?.IsBullet() ?? false;
    }
}