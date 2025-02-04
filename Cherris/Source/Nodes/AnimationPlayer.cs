using System.Reflection;

namespace Cherris;

public class AnimationPlayer : Node
{
    public Animation? DefaultAnimation { get; set; }
    public bool AutoPlay { get; set; } = false;

    private Animation? currentAnimation;
    private float animationTime;
    private bool playing;

    // Main

    public override void Ready()
    {
        if (AutoPlay)
        {
            if (DefaultAnimation != null)
            {
                Play(DefaultAnimation);
            }
            else
            {
                Log.Error("[AnimationPlayer] Ready: AutoPlay is enabled but DefaultAnimation is null.");
            }
        }
    }

    public override void Update()
    {
        if (!playing || currentAnimation == null || currentAnimation.Keyframes.Count == 0)
        {
            return;
        }

        animationTime += TimeServer.Delta;
        var (prev, next) = GetKeyframes(animationTime);
        if (prev == null || next == null)
        {
            playing = false;
            Console.WriteLine($"Animation finished in {animationTime} seconds");
            return;
        }

        float t = float.Clamp((animationTime - prev.Time) / (next.Time - prev.Time), 0, 1);

        foreach (var (nodePath, prevProps) in prev.Nodes)
        {
            if (!next.Nodes.TryGetValue(nodePath, out var nextProps))
            {
                continue;
            }

            Node node = GetNode<Node>(nodePath);
            if (node == null)
            {
                continue;
            }

            foreach (var (propertyName, prevValues) in prevProps)
            {
                if (!nextProps.TryGetValue(propertyName, out var nextValues))
                {
                    continue;
                }

                SetAnimatedProperty(node, propertyName, prevValues, nextValues, t);
            }
        }
    }

    // Public

    public void Play(string name)
    {
        Animation animation = ResourceLoader.Load<Animation>(name);
        Play(animation);
    }

    public void Play(Animation animation)
    {
        currentAnimation = animation;
        animationTime = 0f;
        playing = true;
    }

    // Utils

    private static void SetAnimatedProperty(Node node, string propertyName, Dictionary<string, float> prevComponents, Dictionary<string, float> nextComponents, float t)
    {
        MemberInfo member = GetMember(node.GetType(), propertyName);
        Type memberType = GetMemberType(member);

        object interpolatedValue = CreateInterpolatedValue(memberType, prevComponents, nextComponents, t);
        SetMemberValue(member, node, interpolatedValue);
    }

    private static object CreateInterpolatedValue(Type type, Dictionary<string, float> prevComponents, Dictionary<string, float> nextComponents, float t)
    {
        if (type == typeof(float))
        {
            return float.Lerp(prevComponents[""], nextComponents[""], t);
        }

        object instance = Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Cannot create instance of type '{type}'");

        foreach (var (componentName, prevValue) in prevComponents)
        {
            if (!nextComponents.TryGetValue(componentName, out float nextValue))
            {
                continue;
            }

            MemberInfo member = GetMember(type, componentName);
            Type memberType = GetMemberType(member);
            object interpolatedComponent = CreateInterpolatedValue(memberType, new() { [""] = prevValue }, new() { [""] = nextValue }, t);

            SetMemberValue(member, instance, interpolatedComponent);
        }

        return instance;
    }
    
    private static MemberInfo GetMember(Type type, string name)
    {
        // Attempt to get the field first
        FieldInfo? field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            return field;
        }

        // Attempt to get the property if the field wasn't found
        PropertyInfo? property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        if (property != null)
        {
            return property;
        }

        // If neither are found, throw an exception
        throw new InvalidOperationException($"Member '{name}' not found in type '{type}'");
    }

    private static void SetMemberValue(MemberInfo member, object target, object value)
    {
        switch (member.MemberType)
        {
            case MemberTypes.Field:
                ((FieldInfo)member).SetValue(target, value);
                break;
            case MemberTypes.Property:
                ((PropertyInfo)member).SetValue(target, value);
                break;
            default:
                throw new InvalidOperationException("Unsupported member type");
        }
    }

    private static Type GetMemberType(MemberInfo member)
    {
        return member.MemberType switch
        {
            MemberTypes.Field => ((FieldInfo)member).FieldType,
            MemberTypes.Property => ((PropertyInfo)member).PropertyType,
            _ => throw new InvalidOperationException("Unsupported member type")
        };
    }

    private (Animation.Keyframe? prev, Animation.Keyframe? next) GetKeyframes(float time)
    {
        if (currentAnimation == null)
        {
            return (null, null);
        }

        Animation.Keyframe? prev = null;
        Animation.Keyframe? next = null;
        foreach (Animation.Keyframe keyframe in currentAnimation.Keyframes)
        {
            if (keyframe.Time <= time)
            {
                prev = keyframe;
            }
            else
            {
                next = keyframe;
                break;
            }
        }

        return (prev, next);
    }
}