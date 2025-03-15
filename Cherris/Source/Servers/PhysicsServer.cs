using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.World;
using static Box2D.NetStandard.Dynamics.World.World;

namespace Cherris;

public sealed class PhysicsServer
{
    private static PhysicsServer? _instance;
    public static PhysicsServer Instance => _instance ??= new();

    public List<RigidBody> RigidBodies = [];
    public readonly Dictionary<RigidBody, Body> Bodies = [];

    private readonly World world;

    private const int VelocityIterations = 6;
    private const int PositionIterations = 2;

    private PhysicsServer()
    {
        world = new(new(0, 9.8f));
    }

    public void Process()
    {
        world.Step(TimeServer.Delta, VelocityIterations, PositionIterations);
        SyncRigidBodyStates();
    }

    public void Register(RigidBody rigidBody)
    {
        RigidBodies.Add(rigidBody);
        CreateBox2DBody(rigidBody);
    }

    public void Unregister(RigidBody rigidBody)
    {
        if (Bodies.TryGetValue(rigidBody, out Body? body))
        {
            world.DestroyBody(body);
            Bodies.Remove(rigidBody);
        }

        RigidBodies.Remove(rigidBody);
    }

    private void SyncRigidBodyStates()
    {
        foreach (var kvp in Bodies)
        {
            RigidBody rb = kvp.Key;
            Body body = kvp.Value;

            rb.InheritPosition = false;
            rb.GlobalPosition = new(body.GetPosition().X, body.GetPosition().Y);
            rb.Rotation = body.GetAngle();
        }
    }

    public void PerformRayCast(RayCastCallback callback, in Vector2 point1, in Vector2 point2)
    {
        world.RayCast(callback, point1, point2);
    }

    private void CreateBox2DBody(RigidBody rigidBody)
    {
        Body body = world.CreateBody(new()
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
        });

        Bodies[rigidBody] = body;
        rigidBody.Box2DBody = body;
        CreateBox2DFixture(rigidBody.Collider, body);
    }

    private static void CreateBox2DFixture(BoxCollider collider, Body body)
    {
        PolygonShape shape = new();
        shape.SetAsBox(collider.Size.X / 2, collider.Size.Y / 2, new(collider.Offset.X, collider.Offset.Y), 0);

        body.CreateFixture(new()
        {
            shape = shape,
            density = collider.Density,
            friction = collider.Friction,
            restitution = collider.Restitution,
        });
    }
}