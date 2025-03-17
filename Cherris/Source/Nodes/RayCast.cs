using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using System.Numerics;
using System.Linq;

namespace Cherris;

public class RayCast : Node2D
{
    public List<int> CollisionLayers { get; set; } = [0];
    public float Length { get; set; } = 100f;
    public bool IgnoreFirst { get; set; } = false;
    public bool IsColliding { get; private set; }
    public Vector2 CollisionPoint { get; private set; }
    public RigidBody? Collider { get; private set; } // Collider is of type RigidBody now

    public Vector2 TargetPosition => new Vector2(
        float.Cos(Rotation * MathF.PI / 180f),
        float.Sin(Rotation * MathF.PI / 180f)
    ) * Length;

    public override void Process()
    {
        base.Process();

        Perform();
    }

    private void Perform()
    {
        IsColliding = false;
        Collider = null;
        CollisionPoint = Vector2.Zero;

        Vector2 rayStart = GlobalPosition;
        Vector2 rayEnd = rayStart + TargetPosition;

        RayCastCallbackHandler callbackHandler = new();

        World.RayCastCallback callback = callbackHandler.ReportFixture;

        PhysicsServer.Instance.PerformRayCast(callback, rayStart, rayEnd);

        foreach (var hit in callbackHandler.Hits.OrderBy(h => h.Fraction))
        {
            Fixture fixture = hit.Fixture;
            Body? body = fixture.Body;

            Node? node = PhysicsServer.Instance.FindNodeFromBody(body); // Use FindNodeFromBody to get Node?

            if (node is null) // Null check for node
            {
                continue;
            }

            if (node is not RigidBody rigidBody) // Check if node is a RigidBody and cast
            {
                continue; // If it's not a RigidBody, skip it (or handle differently if needed)
            }

            if (!rigidBody.Collider.Enabled) // Now safe to access rigidBody.Collider because of the cast
            {
                continue;
            }

            if (!CollisionLayers.Intersect(rigidBody.Collider.CollisionLayers).Any()) // Again, safe to access rigidBody.Collider
            {
                continue;
            }

            if (IgnoreFirst)
            {
                IgnoreFirst = false;
                continue;
            }

            IsColliding = true;
            Collider = rigidBody; // Now Collider is correctly assigned as RigidBody?
            CollisionPoint = hit.Point;
            Log.Info("Hit: " + Collider.Name);
            return;
        }
    }

    private class RayCastCallbackHandler
    {
        public List<RayCastHit> Hits { get; } = [];

        public void ReportFixture(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            Hits.Add(new()
            {
                Fixture = fixture,
                Point = point,
                Fraction = fraction
            });
        }
    }

    private struct RayCastHit
    {
        public Fixture Fixture { get; set; }
        public Vector2 Point { get; set; }
        public float Fraction { get; set; }
    }
}