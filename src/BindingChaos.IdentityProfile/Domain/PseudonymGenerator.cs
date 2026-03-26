namespace BindingChaos.IdentityProfile.Domain;

internal static class PseudonymGenerator
{
    private static readonly IReadOnlyList<string> IngVerbs = new[]
    {
        "Sparking", "Growing", "Flowing", "Glowing", "Shining", "Rising", "Falling", "Running", "Walking", "Flying",
        "Dancing", "Singing", "Dreaming", "Hoping", "Loving", "Caring", "Helping", "Guiding", "Teaching", "Learning",
        "Creating", "Building", "Making", "Finding", "Seeking", "Knowing", "Thinking", "Feeling", "Seeing", "Hearing",
        "Soaring", "Leaping", "Gliding", "Blooming", "Flourishing", "Embracing", "Inspiring", "Exploring", "Discovering", "Wandering",
        "Whispering", "Laughing", "Smiling", "Trusting", "Believing", "Imagining", "Wondering", "Pondering", "Reflecting", "Meditating",
        "Breathing", "Focusing", "Balancing", "Connecting", "Uniting", "Sharing", "Giving", "Receiving", "Offering", "Welcoming",
    };

    private static readonly IReadOnlyList<string> Adjectives = new[]
    {
        "Brave", "Swift", "Wise", "Bright", "Calm", "Clear", "Deep", "Fair", "Gentle", "Kind",
        "Light", "Pure", "Quiet", "Sharp", "Soft", "Strong", "Warm", "Wild", "Young", "Old",
        "New", "Fresh", "Cool", "Dark", "Golden", "Silver", "Crystal", "Hidden", "Open", "Bold",
        "Clever", "Noble", "Fierce", "Quick", "Smooth", "Ancient", "Sacred", "Mystic", "Royal", "Serene",
        "Vibrant", "Tender", "Mighty", "Peaceful", "Radiant", "Steady", "Graceful", "Loyal", "Honest", "Patient",
        "Humble", "Joyful", "Curious", "Creative", "Focused", "Balanced", "Hopeful", "Cheerful", "Silent", "Fearless",
    };

    private static readonly IReadOnlyList<string> Nouns = new[]
    {
        "Lion", "River", "Oak", "Star", "Moon", "Sun", "Wind", "Fire", "Water", "Earth",
        "Mountain", "Forest", "Ocean", "Sky", "Cloud", "Rain", "Snow", "Ice", "Stone", "Metal",
        "Crystal", "Gem", "Flower", "Tree", "Bird", "Wolf", "Bear", "Deer", "Fish", "Dragon",
        "Phoenix", "Eagle", "Hawk", "Owl", "Raven", "Swan", "Dove", "Sparrow", "Robin", "Bluejay",
        "Tiger", "Whale", "Salmon", "Falcon", "Panther", "Butterfly", "Rose", "Lily", "Orchid", "Jasmine",
        "Willow", "Cedar", "Maple", "Pine", "Bamboo", "Coral", "Pearl", "Amber", "Jade", "Ruby",
        "Emerald", "Diamond", "Opal", "Quartz", "Marble", "Granite", "Thunder", "Lightning", "Mist", "Dawn",
        "Dusk", "Aurora", "Comet", "Galaxy", "Nebula", "Planet", "Meteor", "Cosmos", "Horizon", "Valley",
    };

    /// <summary>
    /// Generates a pseudonym in IngVerbAdjectiveNoun format using the provided random instance.
    /// </summary>
    /// <param name="random">The random instance used for word selection.</param>
    /// <returns>A pseudonym such as "SeekingBrightRiver".</returns>
    internal static string Generate(Random random)
    {
        var verb = IngVerbs[random.Next(IngVerbs.Count)];
        var adjective = Adjectives[random.Next(Adjectives.Count)];
        var noun = Nouns[random.Next(Nouns.Count)];
        return $"{verb}{adjective}{noun}";
    }
}
