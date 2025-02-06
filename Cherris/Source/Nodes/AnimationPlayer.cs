using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cherris;

public class AnimationPlayer : Node
{
    public Animation? DefaultAnimation { get; set; }
    public bool AutoPlay { get; set; }
    public bool IsPlaying => playing;

    private Animation? currentAnimation;
    private float animationTime;
    private bool playing;

    public override void Ready()
    {
        if (AutoPlay && DefaultAnimation != null)
        {
            Play(DefaultAnimation);
        }
    }

    public override void Update()
    {
        if (!playing || currentAnimation == null) return;

        animationTime += TimeServer.Delta;
        var (prev, next) = GetKeyframes(animationTime);

        if (prev == null || next == null)
        {
            Stop();
            return;
        }

        float t = float.Clamp((animationTime - prev.Time) / (next.Time - prev.Time), 0, 1);
        AnimateBetweenKeyframes(prev, next, t);
    }

    public void Play(string name) => Play(ResourceLoader.Load<Animation>(name));

    public void Play(Animation animation)
    {
        currentAnimation = animation;
        animationTime = 0f;
        playing = true;
        Console.WriteLine($"[AnimationPlayer] Started animation: {animation}");
    }

    public void Stop()
    {
        playing = false;
        currentAnimation = null;
        animationTime = 0f;
        Console.WriteLine("[AnimationPlayer] Animation stopped");
    }

    private void AnimateBetweenKeyframes(Animation.Keyframe prev, Animation.Keyframe next, float t)
    {
        foreach (var (nodePath, prevProps) in prev.Nodes)
        {
            if (!next.Nodes.TryGetValue(nodePath, out var nextProps)) continue;

            Node? node = GetNode<Node>(nodePath);
            if (node == null) continue;

            foreach (var (propertyPath, prevValue) in prevProps)
            {
                if (!nextProps.TryGetValue(propertyPath, out float nextValue)) continue;
                SetAnimatedProperty(node, propertyPath, prevValue, nextValue, t);
            }
        }
    }

    private void SetAnimatedProperty(Node node, string propertyPath, float prevValue, float nextValue, float t)
    {
        try
        {
            string[] parts = propertyPath.Split('/');
            object currentObj = node;
            var hierarchy = new List<(object Parent, MemberInfo Member)>();
            object? modifiedChild = null;

            // Traverse property hierarchy
            for (int i = 0; i < parts.Length - 1; i++)
            {
                MemberInfo member = GetMember(currentObj.GetType(), parts[i]);
                hierarchy.Add((currentObj, member));
                currentObj = GetMemberValue(member, currentObj);
            }

            // Set final value
            MemberInfo finalMember = GetMember(currentObj.GetType(), parts[^1]);
            Type memberType = GetMemberType(finalMember);
            object value = CreateInterpolatedValue(memberType, parts[^1], prevValue, nextValue, t);

            SetMemberValue(finalMember, currentObj, value);
            modifiedChild = currentObj;

            // Propagate changes back through hierarchy
            for (int i = hierarchy.Count - 1; i >= 0; i--)
            {
                var (parent, member) = hierarchy[i];
                SetMemberValue(member, parent, modifiedChild!);
                modifiedChild = GetMemberValue(member, parent);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AnimationPlayer] Error: {ex}");
            Stop();
        }
    }

    private object CreateInterpolatedValue(Type type, string component, float prev, float next, float t)
    {
        if (type == typeof(float)) return float.Lerp(prev, next, t);

        object instance = Activator.CreateInstance(type)!;
        float value = float.Lerp(prev, next, t);

        // Try to set component
        FieldInfo? field = type.GetField(component) ?? TryFindComponentField(type);
        PropertyInfo? prop = type.GetProperty(component) ?? TryFindComponentProperty(type);

        if (field != null && field.FieldType == typeof(float))
        {
            field.SetValue(instance, value);
        }
        else if (prop != null && prop.PropertyType == typeof(float))
        {
            prop.SetValue(instance, value);
        }

        return instance;
    }

    private FieldInfo? TryFindComponentField(Type type)
    {
        return type.GetField("X") ?? type.GetField("Y") ?? type.GetField("Z") ?? type.GetField("W");
    }

    private PropertyInfo? TryFindComponentProperty(Type type)
    {
        return type.GetProperty("X") ?? type.GetProperty("Y") ?? type.GetProperty("Z") ?? type.GetProperty("W");
    }

    private (Animation.Keyframe? prev, Animation.Keyframe? next) GetKeyframes(float time)
    {
        Animation.Keyframe? prev = null;
        Animation.Keyframe? next = null;

        foreach (var keyframe in currentAnimation?.Keyframes ?? Enumerable.Empty<Animation.Keyframe>())
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

    private static MemberInfo GetMember(Type type, string name)
    {
        return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) as MemberInfo
            ?? type.GetField(name, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new ArgumentException($"Member '{name}' not found in {type.Name}");
    }

    private static object GetMemberValue(MemberInfo member, object target)
    {
        return member.MemberType switch
        {
            MemberTypes.Property => ((PropertyInfo)member).GetValue(target)!,
            MemberTypes.Field => ((FieldInfo)member).GetValue(target)!,
            _ => throw new InvalidOperationException("Unsupported member type")
        };
    }

    private static void SetMemberValue(MemberInfo member, object target, object value)
    {
        switch (member.MemberType)
        {
            case MemberTypes.Property:
                ((PropertyInfo)member).SetValue(target, value);
                break;
            case MemberTypes.Field:
                ((FieldInfo)member).SetValue(target, value);
                break;
            default:
                throw new InvalidOperationException("Unsupported member type");
        }
    }

    private static Type GetMemberType(MemberInfo member)
    {
        return member.MemberType switch
        {
            MemberTypes.Property => ((PropertyInfo)member).PropertyType,
            MemberTypes.Field => ((FieldInfo)member).FieldType,
            _ => throw new InvalidOperationException("Unsupported member type")
        };
    }
}