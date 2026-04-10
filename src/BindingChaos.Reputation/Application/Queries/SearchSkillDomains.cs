using BindingChaos.Reputation.Application.ReadModels;
using BindingChaos.Reputation.Domain.SkillDomains;

namespace BindingChaos.Reputation.Application.Queries;

/// <summary>
/// Query to search the skill domain catalogue by localized name.
/// </summary>
/// <param name="Query">The search term.</param>
/// <param name="Locale">Preferred BCP 47 locale (e.g. <c>sv</c>). Defaults to <c>en</c>.</param>
public sealed record SearchSkillDomains(string Query, string Locale = "en");

/// <summary>
/// Handles the <see cref="SearchSkillDomains"/> query.
/// </summary>
public static class SearchSkillDomainsHandler
{
    /// <summary>
    /// Returns skill domains whose localized name matches the search term.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="repository">The skill domain repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The matching domains.</returns>
    public static async Task<IReadOnlyList<SkillDomainView>> Handle(
        SearchSkillDomains query,
        ISkillDomainRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var domains = await repository.SearchAsync(query.Query, query.Locale, cancellationToken).ConfigureAwait(false);

        return domains.Select(domain =>
        {
            var localization = domain.Localizations.FirstOrDefault(l => l.Locale == query.Locale)
                ?? domain.Localizations.FirstOrDefault(l => l.Locale == "en")
                ?? domain.Localizations.FirstOrDefault();

            return localization is null
                ? null
                : new SkillDomainView(domain.Id, domain.Slug, localization.Name, localization.Description, localization.Locale);
        }).OfType<SkillDomainView>().ToList();
    }
}
