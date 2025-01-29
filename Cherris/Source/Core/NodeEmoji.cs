namespace Cherris;

public static class NodeEmoji
{
    public static string GetEmojiForNodeType(Node node)
    {
        return node switch
        {
            ParticleGenerator => "✨",
            Slider => "🎚️",
            Label => "💬",
            Camera => "🎥",
            RayCast => "⚡",
            Timer => "⏰",
            NavigationRegion => "🗺",
            NavigationAgent => "🧭",
            AudioPlayer => "🔉",
            Collider => "📦",
            Sprite => "🖼",
            Button => "🔘",
            ColorRectangle => "🟥",
            Node2D => "🟩",
            _ => "⭕",
        };
    }
}
