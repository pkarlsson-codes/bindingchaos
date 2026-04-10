namespace BindingChaos.Reputation.Application.ReadModels;

/// <summary>A localized view of a skill domain.</summary>
/// <param name="Id">The domain ID.</param>
/// <param name="Slug">The globally unique ASCII slug (e.g. <c>engineering</c>).</param>
/// <param name="Name">The localized name.</param>
/// <param name="Description">The localized description, if any.</param>
/// <param name="Locale">The locale this view is rendered in.</param>
public sealed record SkillDomainView(Guid Id, string Slug, string Name, string? Description, string Locale);
