using BindingChaos.Reputation.Application.ReadModels;
using BindingChaos.Reputation.Domain.SkillEndorsements;
using BindingChaos.Reputation.Domain.Skills;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Application.Queries;

/// <summary>
/// Query to retrieve the competency profile of a participant.
/// </summary>
/// <param name="ParticipantId">The participant to query.</param>
/// <param name="Locale">Preferred BCP 47 locale for skill names. Defaults to <c>en</c>.</param>
public sealed record GetParticipantCompetencies(ParticipantId ParticipantId, string Locale = "en");

/// <summary>
/// Handles the <see cref="GetParticipantCompetencies"/> query.
/// </summary>
public static class GetParticipantCompetenciesHandler
{
    /// <summary>
    /// Returns the participant's endorsed skills with counts and average grades.
    /// Enriches graph results with localized skill names from the catalogue.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="competenceGraph">The competence graph query service.</param>
    /// <param name="skillRepository">The skill repository for name lookup.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The participant's competency profile.</returns>
    public static async Task<IReadOnlyList<ParticipantCompetencyView>> Handle(
        GetParticipantCompetencies query,
        ICompetenceGraphQueryService competenceGraph,
        ISkillRepository skillRepository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var competencies = await competenceGraph
            .GetParticipantCompetenciesAsync(query.ParticipantId, cancellationToken)
            .ConfigureAwait(false);

        var views = new List<ParticipantCompetencyView>(competencies.Count);
        foreach (var c in competencies)
        {
            var skill = await skillRepository.GetByIdAsync(c.SkillId, cancellationToken).ConfigureAwait(false);
            if (skill is null || skill.Domain is null)
            {
                continue;
            }

            var localization = skill.Localizations.FirstOrDefault(l => l.Locale == query.Locale)
                ?? skill.Localizations.FirstOrDefault(l => l.Locale == "en")
                ?? skill.Localizations.FirstOrDefault();

            if (localization is null)
            {
                continue;
            }

            views.Add(new ParticipantCompetencyView(
                skill.Id,
                skill.Domain.Slug,
                skill.Slug,
                localization.Name,
                c.EndorsementCount,
                c.AverageGrade));
        }

        return views;
    }
}
