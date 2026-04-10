namespace BindingChaos.Reputation.Domain.Skills;

/// <summary>
/// A localized name and description for a skill in a specific locale.
/// </summary>
public sealed class SkillLocalization
{
    /// <summary>Gets the BCP 47 locale code (e.g. <c>en</c>, <c>sv</c>, <c>zh-Hans</c>).</summary>
    required public string Locale { get; init; }

    /// <summary>Gets the localized skill name.</summary>
    required public string Name { get; init; }

    /// <summary>Gets an optional localized description of the skill.</summary>
    public string? Description { get; init; }
}
