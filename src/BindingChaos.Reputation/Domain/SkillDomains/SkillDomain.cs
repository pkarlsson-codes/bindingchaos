namespace BindingChaos.Reputation.Domain.SkillDomains;

/// <summary>
/// A first-class category that skills belong to (e.g. <c>engineering</c>, <c>music</c>).
/// Identity is the <see cref="Id"/>; the <see cref="Slug"/> is a globally unique, URL-safe
/// ASCII identifier that may be updated without breaking skill references.
/// </summary>
public sealed class SkillDomain
{
    /// <summary>Gets the unique identifier for this domain.</summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the globally unique, ASCII slug (e.g. <c>engineering</c>, <c>creative-arts</c>).
    /// </summary>
    required public string Slug { get; init; }

    /// <summary>Gets the UTC timestamp when this domain was created.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets the localized names and descriptions for this domain.</summary>
    public List<SkillDomainLocalization> Localizations { get; init; } = [];
}
