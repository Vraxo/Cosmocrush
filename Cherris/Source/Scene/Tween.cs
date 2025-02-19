using System.Reflection;

namespace Cherris;

public class Tween(Node creatorNode, Node.ProcessMode processMode = Node.ProcessMode.Inherit)
{
    public bool Active = true;

    private readonly List<TweenStep> steps = [];
    private readonly Node creatorNode = creatorNode;
    private readonly Node.ProcessMode processMode = processMode;

    private static readonly bool debug = false;
    
    public bool Stopped { get; private set; }

    public void Stop()
    {
        Stopped = true;
        Active = false;
    }

    public void TweenProperty(Node node, string propertyPath, float targetValue, float duration)
    {
        try
        {
            Log.Info($"[Tween] Starting tween on {node.Name} for {propertyPath}", debug);
            float startValue = GetFloatValue(node, propertyPath);
            Log.Info($"[Tween] Start value: {startValue} ➔ Target: {targetValue} ({duration}s)", debug);

            steps.Add(new(node, propertyPath, startValue, targetValue, duration));
        }
        catch (Exception ex)
        {
            Log.Error($"[Tween] Error starting tween: {ex}");
            Active = false;
        }
    }

    public void Update(float delta)
    {
        if (!Active || Stopped)
        {
            return;
        }

        foreach (TweenStep step in steps.ToList())
        {
            step.Elapsed += delta;
            var t = float.Clamp(step.Elapsed / step.Duration, 0, 1);
            float currentValue = step.StartValue + (step.EndValue - step.StartValue) * t;

           Log.Info($"[Tween] Updating {step.Node.Name}.{step.PropertyPath} {currentValue:0.00} ({t:P0})", debug);

            SetFloatValueDirect(step.Node, step.PropertyPath, currentValue);

            if (step.Elapsed >= step.Duration)
            {
                Log.Info($"[Tween] Completed {step.Node.Name}.{step.PropertyPath}", debug);
                steps.Remove(step);
            }
        }

        if (steps.Count == 0)
        {
            Log.Info("[Tween] All steps completed", debug);
            Active = false;
        }
    }

    public bool ShouldProcess(bool treePaused)
    {
        var effectiveMode = processMode == Node.ProcessMode.Inherit
            ? GetEffectiveProcessMode(creatorNode)
            : processMode;

        return effectiveMode switch
        {
            Node.ProcessMode.Disabled => false,
            Node.ProcessMode.Always => true,
            Node.ProcessMode.Pausable => !treePaused,
            Node.ProcessMode.WhenPaused => treePaused,
            _ => false
        };
    }

    private static Node.ProcessMode GetEffectiveProcessMode(Node node)
    {
        Node? current = node;

        while (current != null)
        {
            if (current.ProcessingMode != Node.ProcessMode.Inherit)
            {
                return current.ProcessingMode;
            }

            current = current.Parent;
        }

        return Node.ProcessMode.Pausable;
    }

    private static float GetFloatValue(Node node, string propertyPath)
    {
        object? current = node;

        foreach (string part in propertyPath.Split('/'))
        {
            var member = ((MemberInfo?)current?.GetType().GetProperty(part)
                              ?? current?.GetType().GetField(part)) ?? throw new ArgumentException($"Property or field '{part}' not found in {current.GetType().Name}");

            current = member.MemberType == MemberTypes.Property
                ? ((PropertyInfo)member).GetValue(current)
                : ((FieldInfo)member).GetValue(current);
        }
        return (float)current;
    }

    private void SetFloatValueDirect(Node node, string propertyPath, float value)
    {
        string[] parts = propertyPath.Split('/');
        object current = node;

        // Traverse to parent object (but don't modify parents)
        for (int i = 0; i < parts.Length - 1; i++)
        {
            MemberInfo member = ((MemberInfo)current.GetType().GetProperty(parts[i])
                               ?? current.GetType().GetField(parts[i])) ?? throw new ArgumentException($"Property or field '{parts[i]}' not found in {current.GetType().Name}");

            current = member.MemberType == MemberTypes.Property
                ? ((PropertyInfo)member).GetValue(current)
                : ((FieldInfo)member).GetValue(current);
        }

        // Set final value directly
        var finalMember = ((MemberInfo)current.GetType().GetProperty(parts[^1])
                                ?? current.GetType().GetField(parts[^1])) ?? throw new ArgumentException($"Property or field '{parts[^1]}' not found in {current.GetType().Name}");

        if (finalMember.MemberType == MemberTypes.Property)
        {
            ((PropertyInfo)finalMember).SetValue(current, value);
        }
        else
        {
            ((FieldInfo)finalMember).SetValue(current, value);
        }
    }

    private record TweenStep(Node Node, string PropertyPath, float StartValue, float EndValue, float Duration)
    {
        public float Elapsed { get; set; }
    }
}