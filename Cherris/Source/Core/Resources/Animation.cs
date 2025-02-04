using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public class Animation
{
    public List<Keyframe> Keyframes { get; set; } = new();

    public Animation() { }

    public Animation(string filePath)
    {
        string yamlContent = File.ReadAllText(filePath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance) // 🔑 Critical change
            .Build();

        Keyframes = deserializer.Deserialize<List<Keyframe>>(yamlContent);
    }

    public class Keyframe
    {
        [YamlMember(Alias = "T")] // Now works as expected
        public float Time { get; set; }

        [YamlMember]
        public Dictionary<string, Dictionary<string, Dictionary<string, float>>> Nodes { get; set; } = new();
    }
}