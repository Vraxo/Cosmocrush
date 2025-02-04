﻿namespace Cherris;

public abstract class Collider : Node2D
{
    public bool IsStatic { get; set; } = false;
    public List<int> CollisionLayers { get; set; } = [0];
    public Color Color { get; set; } = new(0, 0, 128, 128);

    public override void Ready()
    {
        base.Ready();

        Visible = true;
    }

    public abstract bool RayIntersects(Vector2 rayStart, Vector2 rayEnd);
}