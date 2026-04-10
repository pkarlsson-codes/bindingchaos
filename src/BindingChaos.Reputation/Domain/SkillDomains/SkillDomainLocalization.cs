namespace BindingChaos.Reputation.Domain.SkillDomains;

/// <summary>
/// A localized name and optional description for a <see cref="SkillDomain"/> in a specific locale.
/// </summary>
public sealed class SkillDomainLocalization
{
    /// <summary>Gets the BCP 47 locale code (e.g. <c>en</c>, <c>sv</c>).</summary>
    required public string Locale { get; init; }

    /// <summary>Gets the localized name of the domain.</summary>
    required public string Name { get; init; }

    /// <summary>Gets an optional localized description of the domain.</summary>
    public string? Description { get; init; }
}
