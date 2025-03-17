using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Contacts;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using static Box2D.NetStandard.Dynamics.World.World;

namespace Cherris;

public sealed class PhysicsServer
{
    private static PhysicsServer? _instance;
    public static PhysicsServer Instance => _instance ??= new();

    public List<RigidBody> RigidBodies = [];
    public List<Area2D> Area2Ds = [];
    public readonly Dictionary<Node, Body> Bodies = [];

    private readonly World world;
    private readonly ContactListener contactListener;

    // Add a list to track nodes pending destruction
    private List<Node> _pendingFreeNodes = new();

    private const int VelocityIterations = 6;
    private const int PositionIterations = 2;

    private PhysicsServer()
    {
        world = new World(new(0, 9.8f));
        contactListener = new ContactListener();
        world.SetContactListener(contactListener);
        contactListener.PhysicsServerInstance = this;
    }

    public void Process()
    {
        // Step the physics world
        world.Step(TimeServer.Delta, VelocityIterations, PositionIterations);

        // Sync RigidBody and Area2D states
        SyncRigidBodyStates();
        SyncArea2DStates();

        // Destroy pending nodes AFTER the physics step
        foreach (var node in _pendingFreeNodes)
        {
            if (node is Area2D area) DestroyBox2DAreaBody(area);
            if (node is RigidBody rb) DestroyBox2DBody(rb);
        }
        _pendingFreeNodes.Clear();
    }

    // Add a method to queue nodes for deferred destruction
    public void QueueFreeNode(Node node)
    {
        _pendingFreeNodes.Add(node);
    }

    public void Register(RigidBody rigidBody)
    {
        RigidBodies.Add(rigidBody);
        if (rigidBody.Enabled)
        {
            CreateBox2DBody(rigidBody);
        }
    }

    public void Unregister(RigidBody rigidBody)
    {
        if (rigidBody.Enabled)
        {
            DestroyBox2DBody(rigidBody);
        }
        RigidBodies.Remove(rigidBody);
    }

    internal void CreateBox2DBody(RigidBody rigidBody)
    {
        if (Bodies.ContainsKey(rigidBody))
        {
            DestroyBox2DBody(rigidBody);
        }

        BodyDef bodyDef = new()
        {
            type = rigidBody.Type switch
            {
                RigidBody.BodyType.Static => BodyType.Static,
                RigidBody.BodyType.Dynamic => BodyType.Dynamic,
                RigidBody.BodyType.Kinematic => BodyType.Kinematic,
                _ => BodyType.Static
            },
            position = new(rigidBody.Position.X, rigidBody.Position.Y),
            angle = rigidBody.Rotation,
            fixedRotation = rigidBody.FixedRotation,
            gravityScale = rigidBody.GravityScale,
            // Disable sleeping for dynamic bodies (or just for the player if needed)
            allowSleep = rigidBody.Type == RigidBody.BodyType.Dynamic ? false : true,
        };

        Body body = world.CreateBody(bodyDef);
        Bodies[rigidBody] = body;
        rigidBody.Box2DBody = body;
        CreateBox2DFixture(rigidBody.Collider, body);
    }

    internal void DestroyBox2DBody(RigidBody rigidBody)
    {
        if (Bodies.TryGetValue(rigidBody, out Body body))
        {
            world.DestroyBody(body);
            Bodies.Remove(rigidBody);
            rigidBody.Box2DBody = null;
        }
    }

    private void SyncRigidBodyStates()
    {
        foreach (var kvp in Bodies)
        {
            if (kvp.Key is RigidBody rb)
            {
                Body body = kvp.Value;
                rb.InheritPosition = false;
                rb.GlobalPosition = new(body.GetPosition().X, body.GetPosition().Y);
                rb.Rotation = body.GetAngle();
            }
        }
    }

    public void CreateBox2DAreaBody(Area2D area)
    {
        Area2Ds.Add(area);
        if (Bodies.ContainsKey(area))
        {
            DestroyBox2DAreaBody(area);
        }

        BodyDef bodyDef = new()
        {
            type = BodyType.Kinematic, // Changed to Kinematic
            position = new(area.GlobalPosition.X, area.GlobalPosition.Y),
            angle = area.Rotation,
            fixedRotation = true,
            gravityScale = 0,
        };

        Body body = world.CreateBody(bodyDef);
        Bodies[area] = body;
        area.Box2DBody = body;
        CreateBox2DFixture(area.Collider, body);
    }

    public void DestroyBox2DAreaBody(Area2D area)
    {
        Area2Ds.Remove(area);
        if (Bodies.TryGetValue(area, out Body body))
        {
            world.DestroyBody(body);
            Bodies.Remove(area);
            area.Box2DBody = null;
        }
    }

    public void SyncAreaBodyTransform(Area2D area)
    {
        if (Bodies.TryGetValue(area, out Body body))
        {
            body.SetTransform(new(area.GlobalPosition.X, area.GlobalPosition.Y), area.Rotation);
        }
    }

    private void SyncArea2DStates()
    {
        foreach (var area in Area2Ds)
        {
            if (Bodies.TryGetValue(area, out Body body))
            {
                area.GlobalPosition = new(body.GetPosition().X, body.GetPosition().Y);
                area.Rotation = body.GetAngle();
            }
        }
    }

    private static void CreateBox2DFixture(BoxCollider collider, Body body)
    {
        PolygonShape shape = new();
        shape.SetAsBox(collider.Size.X / 2, collider.Size.Y / 2, new(collider.Offset.X, collider.Offset.Y), 0);

        FixtureDef fixtureDef = new()
        {
            shape = shape,
            density = collider.Density,
            friction = collider.Friction,
            restitution = collider.Restitution,
            isSensor = collider.IsSensor
        };

        body.CreateFixture(fixtureDef);
    }

    public void PerformRayCast(RayCastCallback callback, in Vector2 point1, in Vector2 point2)
    {
        world.RayCast(callback, point1, point2);
    }

    internal Node? FindNodeFromBody(Body body)
    {
        return Bodies
            .FirstOrDefault(kvp => kvp.Value == body)
            .Key;
    }
}

public class ContactListener : Box2D.NetStandard.Dynamics.World.Callbacks.ContactListener
{
    public PhysicsServer? PhysicsServerInstance { get; set; }

    public override void BeginContact(in Contact contact)
    {
        SafeHandleContact(contact, true);
    }

    public override void EndContact(in Contact contact)
    {
        SafeHandleContact(contact, false);
    }

    private void SafeHandleContact(Contact contact, bool isBeginning)
    {
        try
        {
            var fixtureA = contact.GetFixtureA();
            var fixtureB = contact.GetFixtureB();

            Node? nodeA = PhysicsServerInstance?.FindNodeFromBody(fixtureA.Body);
            Node? nodeB = PhysicsServerInstance?.FindNodeFromBody(fixtureB.Body);

            if (nodeA is Area2D areaA)
            {
                HandleAreaContact(areaA, nodeB, isBeginning);
            }
            if (nodeB is Area2D areaB)
            {
                HandleAreaContact(areaB, nodeA, isBeginning);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error handling contact: {e}");
        }
    }

    private static void HandleAreaContact(Area2D area, Node? other, bool isEntering)
    {
        if (area == null || other == null) return;

        if (isEntering)
        {
            area.OnBodyEnteredInternal(other);
        }
        else
        {
            area.OnBodyExitedInternal(other);
        }
    }

    public override void PreSolve(in Contact contact, in Manifold oldManifold) { }
    public override void PostSolve(in Contact contact, in ContactImpulse impulse) { }
}