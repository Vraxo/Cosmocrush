using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public static class FileLoader
{
    private static readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance) // Matches PascalCase YAML keys to properties
        .Build();

    public static T Load<T>(string filePath) where T : new()
    {
        var yamlContent = File.ReadAllText(filePath);
        var data = _deserializer.Deserialize<Dictionary<string, object>>(yamlContent);

        T instance = new T();
        foreach (var kvp in data)
        {
            PackedSceneUtils.SetNestedProperty(instance, kvp.Key, kvp.Value);
        }

        return instance;
    }
}