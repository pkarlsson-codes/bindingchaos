namespace BindingChaos.Reputation.Application.ReadModels;

/// <summary>A localized view of a skill.</summary>
/// <param name="Id">The skill ID.</param>
/// <param name="DomainId">The ID of the domain this skill belongs to.</param>
/// <param name="DomainSlug">The domain's ASCII slug (e.g. <c>engineering</c>).</param>
/// <param name="Slug">The skill's ASCII slug within its domain (e.g. <c>python</c>).</param>
/// <param name="Name">The localized name.</param>
/// <param name="Description">The localized description, if any.</param>
/// <param name="Locale">The locale this view is rendered in.</param>
public sealed record SkillView(Guid Id, Guid DomainId, string DomainSlug, string Slug, string Name, string? Description, string Locale);
