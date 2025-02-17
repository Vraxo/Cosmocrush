using System;
using System.Collections.Generic;
using System.Reflection;
using static Cherris.Node;

namespace Cherris;

public class Tween
{
    private List<TweenStep> steps = new();
    private bool isActive = true;
    private Node _creatorNode;
    private ProcessMode _processMode;

    public Tween(Node creatorNode, ProcessMode processMode = ProcessMode.Inherit)
    {
        _creatorNode = creatorNode;
        _processMode = processMode;
    }

    public bool IsActive() => isActive;

    // Add this property
    public bool IsStopped { get; private set; }

    // Add this method
    public void Stop()
    {
        IsStopped = true;
        isActive = false; // Prevent further updates
    }

    public void TweenProperty(Node node, string propertyPath, float targetValue, float duration)
    {
        try
        {
            Log.Info($"[Tween] Starting tween on {node.Name} for {propertyPath}");
            float startValue = GetFloatValue(node, propertyPath);
            Log.Info($"[Tween] Start value: {startValue} ➔ Target: {targetValue} ({duration}s)");

            steps.Add(new TweenStep(node, propertyPath, startValue, targetValue, duration));
        }
        catch (Exception ex)
        {
            Log.Error($"[Tween] Error starting tween: {ex}");
            isActive = false;
        }
    }

    public void Update(float delta)
    {
        if (!isActive || IsStopped) return;

        foreach (var step in steps.ToList())
        {
            step.Elapsed += delta;
            float t = Math.Clamp(step.Elapsed / step.Duration, 0, 1);
            float currentValue = step.StartValue + (step.EndValue - step.StartValue) * t;

           // Log.Info($"[Tween] Updating {step.Node.Name}.{step.PropertyPath} " +
                   //$"{currentValue:0.00} ({t:P0})");

            SetFloatValueDirect(step.Node, step.PropertyPath, currentValue);

            if (step.Elapsed >= step.Duration)
            {
                Log.Info($"[Tween] Completed {step.Node.Name}.{step.PropertyPath}");
                steps.Remove(step);
            }
        }

        if (steps.Count == 0)
        {
            //Log.Info("[Tween] All steps completed");
            isActive = false;
        }
    }

    public bool ShouldProcess(bool treePaused)
    {
        ProcessMode effectiveMode = _processMode == ProcessMode.Inherit
            ? GetEffectiveProcessMode(_creatorNode)
            : _processMode;

        return effectiveMode switch
        {
            ProcessMode.Disabled => false,
            ProcessMode.Always => true,
            ProcessMode.Pausable => !treePaused,
            ProcessMode.WhenPaused => treePaused,
            _ => false
        };
    }

    private ProcessMode GetEffectiveProcessMode(Node node)
    {
        Node current = node;
        while (current != null)
        {
            if (current.ProcessingMode != ProcessMode.Inherit)
                return current.ProcessingMode;

            current = current.Parent;
        }
        return ProcessMode.Pausable; // Default if all parents inherit
    }

    private float GetFloatValue(Node node, string propertyPath)
    {
        object current = node;
        foreach (var part in propertyPath.Split('/'))
        {
            MemberInfo member = (MemberInfo)current.GetType().GetProperty(part)
                              ?? current.GetType().GetField(part);
            if (member == null)
                throw new ArgumentException($"Property or field '{part}' not found in {current.GetType().Name}");

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
            MemberInfo member = (MemberInfo)current.GetType().GetProperty(parts[i])
                               ?? current.GetType().GetField(parts[i]);
            if (member == null)
                throw new ArgumentException($"Property or field '{parts[i]}' not found in {current.GetType().Name}");

            current = member.MemberType == MemberTypes.Property
                ? ((PropertyInfo)member).GetValue(current)
                : ((FieldInfo)member).GetValue(current);
        }

        // Set final value directly
        MemberInfo finalMember = (MemberInfo)current.GetType().GetProperty(parts[^1])
                                ?? current.GetType().GetField(parts[^1]);
        if (finalMember == null)
            throw new ArgumentException($"Property or field '{parts[^1]}' not found in {current.GetType().Name}");

        if (finalMember.MemberType == MemberTypes.Property)
            ((PropertyInfo)finalMember).SetValue(current, value);
        else
            ((FieldInfo)finalMember).SetValue(current, value);
    }

    private class TweenStep
    {
        public Node Node { get; }
        public string PropertyPath { get; }
        public float StartValue { get; }
        public float EndValue { get; }
        public float Duration { get; }
        public float Elapsed { get; set; }

        public TweenStep(Node node, string propertyPath, float startValue, float endValue, float duration)
        {
            Node = node;
            PropertyPath = propertyPath;
            StartValue = startValue;
            EndValue = endValue;
            Duration = duration;
        }
    }
}