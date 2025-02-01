using System;
using System.Collections;
using System.IO;
using MessagePack;
using MessagePack.Resolvers;
using YamlDotNet.Serialization;

namespace Cherris;

public static class SceneConverter
{
    public static void ConvertYamlToBinary(string yamlPath, string binaryPath)
    {
        var yamlContent = File.ReadAllText(yamlPath);
        var deserializer = new DeserializerBuilder().Build();
        var yamlData = deserializer.Deserialize<object>(yamlContent);

        var msgpackData = NormalizeData(yamlData);
        var bytes = MessagePackSerializer.Serialize(msgpackData, _options);
        File.WriteAllBytes(binaryPath, bytes);
    }

    private static object NormalizeData(object data)
    {
        switch (data)
        {
            case IDictionary dict:
                var dictionary = new Dictionary<string, object>();
                foreach (DictionaryEntry entry in dict)
                {
                    var key = entry.Key.ToString() ?? throw new InvalidDataException("Dictionary key cannot be null");
                    dictionary[key] = NormalizeData(entry.Value!);
                }
                return dictionary;

            case IList list:
                var array = new List<object>(list.Count);
                foreach (var item in list)
                {
                    array.Add(NormalizeData(item!));
                }
                return array;

            case string s:
                if (decimal.TryParse(s, out var decimalValue))
                    return decimalValue;
                if (bool.TryParse(s, out var boolValue))
                    return boolValue;
                return s;

            default:
                return data;
        }
    }

    private static readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
            NativeDateTimeResolver.Instance,
            StandardResolverAllowPrivate.Instance,
            ContractlessStandardResolver.Instance,
            TypelessObjectResolver.Instance
        ))
        .WithCompression(MessagePackCompression.Lz4BlockArray);
}