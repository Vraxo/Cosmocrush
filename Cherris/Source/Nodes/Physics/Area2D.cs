using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using System.Collections.Generic;
using System.Numerics;
using Box2D.NetStandard.Collision; // Add this at the top
using Box2D.NetStandard.Dynamics.Fixtures;

namespace Cherris;

public class Area2D : CollisionObject2D
{
    public HashSet<Node> OverlappingBodies { get; } = new();
    public Body? Box2DBody { get; set; }

    public event Action<Node>? BodyEntered;
    public event Action<Node>? BodyExited;

    private bool _enabled = true;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value) return;

            _enabled = value;

            if (_enabled)
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

    public Vector2 LinearVelocity
    {
        get => Box2DBody?.GetLinearVelocity() ?? Vector2.Zero;
        set => Box2DBody?.SetLinearVelocity(value);
    }

    public Area2D()
    {
        Collider = (BoxCollider)AddChild(new BoxCollider(), "BoxCollider");
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
        base.Free();
        PhysicsServer.Instance.QueueFreeNode(this);
    }

    public override void Process()
    {
        base.Process();

        if (!Enabled || Box2DBody == null)
        {
            return;
        }

        PhysicsServer.Instance.SyncAreaBodyTransform(this);
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

    public void SetCollisionLayerBit(int bit, bool value)
    {
        if (value)
        {
            CollisionLayer |= (ushort)(1 << bit);
        }
        else
        {
            CollisionLayer &= (ushort)~(1 << bit);
        }

        if (Box2DBody != null)
        {
            UpdateCollisionFilter();
        }
    }

    public void SetCollisionMaskBit(int bit, bool value)
    {
        if (value)
        {
            CollisionMask |= (ushort)(1 << bit);
        }
        else
        {
            CollisionMask &= (ushort)~(1 << bit);
        }

        if (Box2DBody != null)
        {
            UpdateCollisionFilter();
        }
    }

    private void UpdateCollisionFilter()
    {
        if (Box2DBody == null) return;

        Fixture? fixture = Box2DBody.GetFixtureList();
        while (fixture != null)
        {
            // Get current filter
            Filter filter = fixture.FilterData;

            // Modify filter properties
            filter.categoryBits = CollisionLayer;
            filter.maskBits = CollisionMask;

            // Apply modified filter
            fixture.FilterData = filter;
            fixture.Refilter(); // Important: Notify physics system about the change

            fixture = fixture.GetNext();
        }
    }
}