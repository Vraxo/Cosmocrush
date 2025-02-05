﻿using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public class Animation
{
    public List<Keyframe> Keyframes { get; set; } = new();

    public class Keyframe
    {
        [YamlMember(Alias = "T")]
        public float Time { get; set; }

        public Dictionary<string, Dictionary<string, float>> Nodes { get; set; } = new();
    }

    public Animation() { }

    public Animation(string filePath)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        Keyframes = deserializer.Deserialize<List<Keyframe>>(File.ReadAllText(filePath));
        Console.WriteLine($"Loaded animation with {Keyframes.Count} keyframes from {filePath}");
    }
}