namespace Cherris;

public class Node2D : VisualItem
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public OriginPreset OriginPreset { get; set; } = OriginPreset.Center;
    public bool InheritPosition { get; set; } = true;
    public bool InheritOrigin { get; set; } = false;
    public bool InheritScale { get; set; } = true;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
    public float Rotation { get; set; } = 0;

    public Vector2 ScaledSize => Size * Scale;

    public virtual Vector2 Size
    {
        get;
        set
        {
            if (Size == value)
            {
                return;
            }

            field = value;
            SizeChanged?.Invoke(this, Size);
        }
    } = new(320, 320);

    public virtual Vector2 Scale
    {
        get
        {
            if (InheritScale && Parent is Node2D node2DParent)
            {
                return node2DParent.Scale;
            }
            return field;
        }
        set
        {
            field = value;
        }
    } = new(1, 1);

    public Vector2 GlobalPosition
    {
        get
        {
            if (Parent is Node2D parentNode)
            {
                if (InheritPosition)
                {
                    float angleRadians = parentNode.Rotation * (MathF.PI / 180f);
                    float cos = MathF.Cos(angleRadians);
                    float sin = MathF.Sin(angleRadians);

                    Vector2 rotatedPosition = new Vector2(
                        Position.X * cos - Position.Y * sin,
                        Position.X * sin + Position.Y * cos);

                    return parentNode.GlobalPosition + rotatedPosition;
                }

                return field;
            }
            else
            {
                return Position;
            }
        }
        set
        {
            field = value;

            if (Parent is not Node2D)
            {
                Position = value;
            }
        }
    }

    public Vector2 Offset
    {
        get
        {
            if (InheritOrigin)
            {
                if (Parent is Node2D parentNode)
                {
                    return parentNode.Offset;
                }
            }
            return field;
        }
        set
        {
            field = value;
        }
    } = Vector2.Zero;

    public Vector2 Origin
    {
        get
        {
            float x = HorizontalAlignment switch
            {
                HorizontalAlignment.Center => Size.X * Scale.X / 2,
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Right => Size.X,
                _ => 0
            };

            float y = VerticalAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Center => Size.Y * Scale.Y / 2,
                VerticalAlignment.Bottom => Size.Y * Scale.Y,
                _ => 0
            };

            Vector2 alignmentOffset = new(x, y);
            return alignmentOffset + Offset;
        }
    }

    // Events

    public delegate void SizeEvent(Node2D sender, Vector2 size);
    public event SizeEvent? SizeChanged;

    // Methods

    public void LookAt(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - GlobalPosition;
        float angle = MathF.Atan2(direction.Y, direction.X) * 57.29578f; // radians to degrees
        Rotation = angle;
    }
}