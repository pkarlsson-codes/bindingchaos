using BindingChaos.Reputation.Application.ReadModels;
using BindingChaos.Reputation.Domain.Skills;

namespace BindingChaos.Reputation.Application.Queries;

/// <summary>
/// Query to search the skill catalogue by localized name.
/// </summary>
/// <param name="Query">The search term.</param>
/// <param name="Locale">Preferred BCP 47 locale (e.g. <c>sv</c>). Defaults to <c>en</c>.</param>
public sealed record SearchSkills(string Query, string Locale = "en");

/// <summary>
/// Handles the <see cref="SearchSkills"/> query.
/// </summary>
public static class SearchSkillsHandler
{
    /// <summary>
    /// Returns skills whose localized name matches the search term.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="repository">The skill repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The matching skills.</returns>
    public static async Task<IReadOnlyList<SkillView>> Handle(
        SearchSkills query,
        ISkillRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var skills = await repository.SearchAsync(query.Query, query.Locale, cancellationToken).ConfigureAwait(false);

        return skills
            .Where(s => s.Domain is not null)
            .Select(skill =>
            {
                var localization = skill.Localizations.FirstOrDefault(l => l.Locale == query.Locale)
                    ?? skill.Localizations.FirstOrDefault(l => l.Locale == "en")
                    ?? skill.Localizations.FirstOrDefault();

                return localization is null
                    ? null
                    : new SkillView(
                        skill.Id,
                        skill.DomainId,
                        skill.Domain!.Slug,
                        skill.Slug,
                        localization.Name,
                        localization.Description,
                        localization.Locale);
            })
            .OfType<SkillView>()
            .ToList();
    }
}
