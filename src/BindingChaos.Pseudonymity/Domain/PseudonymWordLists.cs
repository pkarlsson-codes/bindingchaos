namespace BindingChaos.Pseudonymity.Domain;

/// <summary>
/// Provides word lists for pseudonym generation using a word-based approach.
/// </summary>
internal static class PseudonymWordLists
{
    /// <summary>
    /// Collection of positive and neutral adjectives for pseudonym generation.
    /// </summary>
    public static readonly IReadOnlyList<string> Adjectives = new[]
    {
        "brave", "swift", "wise", "bright", "calm", "clear", "deep", "fair", "gentle", "kind",
        "light", "pure", "quiet", "sharp", "soft", "strong", "warm", "wild", "young", "old",
        "new", "fresh", "cool", "warm", "dark", "golden", "silver", "crystal", "hidden", "open",
        "bold", "clever", "noble", "fierce", "quick", "smooth", "ancient", "sacred", "mystic", "royal",
        "serene", "vibrant", "tender", "mighty", "peaceful", "radiant", "steady", "graceful", "loyal", "honest",
        "patient", "humble", "joyful", "curious", "creative", "focused", "balanced", "hopeful", "cheerful", "bright"
    };

    /// <summary>
    /// Collection of common nouns for pseudonym generation.
    /// </summary>
    public static readonly IReadOnlyList<string> Nouns = new[]
    {
        "lion", "river", "oak", "star", "moon", "sun", "wind", "fire", "water", "earth",
        "mountain", "forest", "ocean", "sky", "cloud", "rain", "snow", "ice", "stone", "metal",
        "crystal", "gem", "flower", "tree", "bird", "wolf", "bear", "deer", "fish", "dragon",
        "phoenix", "eagle", "hawk", "owl", "raven", "swan", "dove", "sparrow", "robin", "bluejay",
        "tiger", "whale", "salmon", "falcon", "panther", "butterfly", "rose", "lily", "orchid", "jasmine",
        "willow", "cedar", "maple", "pine", "bamboo", "coral", "pearl", "amber", "jade", "ruby",
        "emerald", "diamond", "opal", "quartz", "marble", "granite", "thunder", "lightning", "mist", "dawn",
        "dusk", "aurora", "comet", "galaxy", "nebula", "planet", "meteor", "cosmos", "horizon", "valley"
    };

    /// <summary>
    /// Collection of action verbs for pseudonym generation.
    /// </summary>
    public static readonly IReadOnlyList<string> Verbs = new[]
    {
        "spark", "grow", "flow", "glow", "shine", "rise", "fall", "run", "walk", "fly",
        "dance", "sing", "dream", "hope", "love", "care", "help", "guide", "teach", "learn",
        "create", "build", "make", "find", "seek", "know", "think", "feel", "see", "hear",
        "soar", "leap", "glide", "bloom", "flourish", "embrace", "inspire", "explore", "discover", "wander",
        "whisper", "laugh", "smile", "trust", "believe", "imagine", "wonder", "ponder", "reflect", "meditate",
        "breathe", "focus", "balance", "connect", "unite", "share", "give", "receive", "offer", "welcome"
    };

    /// <summary>
    /// Combined collection of nouns and verbs for the third word position.
    /// Pre-computed to avoid allocations during pseudonym generation.
    /// </summary>
    public static readonly IReadOnlyList<string> CombinedThirdWords = Nouns.Concat(Verbs).ToArray();
}