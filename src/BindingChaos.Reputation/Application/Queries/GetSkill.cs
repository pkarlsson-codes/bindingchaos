using BindingChaos.Reputation.Application.ReadModels;
using BindingChaos.Reputation.Domain.Skills;

namespace BindingChaos.Reputation.Application.Queries;

/// <summary>
/// Query to retrieve a skill by ID with its localized name.
/// Falls back to <c>en</c> if no localization exists for the requested locale.
/// </summary>
/// <param name="SkillId">The skill ID.</param>
/// <param name="Locale">Preferred BCP 47 locale (e.g. <c>sv</c>). Defaults to <c>en</c>.</param>
public sealed record GetSkill(Guid SkillId, string Locale = "en");

/// <summary>
/// Handles the <see cref="GetSkill"/> query.
/// </summary>
public static class GetSkillHandler
{
    /// <summary>
    /// Returns the skill view for the given ID, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="repository">The skill repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The skill view, or <see langword="null"/>.</returns>
    public static async Task<SkillView?> Handle(
        GetSkill query,
        ISkillRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var skill = await repository.GetByIdAsync(query.SkillId, cancellationToken).ConfigureAwait(false);
        if (skill is null || skill.Domain is null)
        {
            return null;
        }

        var localization = skill.Localizations.FirstOrDefault(l => l.Locale == query.Locale)
            ?? skill.Localizations.FirstOrDefault(l => l.Locale == "en")
            ?? skill.Localizations.FirstOrDefault();

        if (localization is null)
        {
            return null;
        }

        return new SkillView(
            skill.Id,
            skill.DomainId,
            skill.Domain.Slug,
            skill.Slug,
            localization.Name,
            localization.Description,
            localization.Locale);
    }
}
