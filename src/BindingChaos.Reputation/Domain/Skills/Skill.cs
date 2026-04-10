using BindingChaos.Reputation.Domain.SkillDomains;

namespace BindingChaos.Reputation.Domain.Skills;

/// <summary>
/// A named competency that participants can be endorsed for.
/// Identity is the <see cref="Id"/> (UUID). The <see cref="Slug"/> is a
/// human-readable identifier scoped to its <see cref="DomainId"/> that may
/// be updated without breaking graph references.
/// </summary>
public sealed class Skill
{
    /// <summary>Gets the unique identifier for this skill.</summary>
    public Guid Id { get; init; }

    /// <summary>Gets the ID of the domain this skill belongs to.</summary>
    public Guid DomainId { get; init; }

    /// <summary>
    /// Gets the ASCII slug for this skill, unique within its domain (e.g. <c>python</c>).
    /// The full canonical path is <c>{domain.Slug}/{Slug}</c>.
    /// </summary>
    required public string Slug { get; init; }

    /// <summary>Gets the UTC timestamp when this skill was created.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets the localized names and descriptions for this skill.</summary>
    public List<SkillLocalization> Localizations { get; init; } = [];

    /// <summary>Gets the domain this skill belongs to. Populated when explicitly loaded.</summary>
    public SkillDomain? Domain { get; set; }
}
