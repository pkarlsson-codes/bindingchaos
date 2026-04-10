using BindingChaos.Reputation.Application.ReadModels;
using BindingChaos.Reputation.Domain.SkillEndorsements;

namespace BindingChaos.Reputation.Application.Queries;

/// <summary>
/// Query to retrieve the top endorsed participants for a given skill.
/// </summary>
/// <param name="SkillId">The skill to query experts for.</param>
/// <param name="Limit">Maximum number of results. Defaults to 20.</param>
public sealed record GetSkillExperts(Guid SkillId, int Limit = 20);

/// <summary>
/// Handles the <see cref="GetSkillExperts"/> query.
/// </summary>
public static class GetSkillExpertsHandler
{
    /// <summary>
    /// Returns participants ranked by total endorsement score for the given skill.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="competenceGraph">The competence graph query service.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ranked list of experts.</returns>
    public static async Task<IReadOnlyList<SkillExpertView>> Handle(
        GetSkillExperts query,
        ICompetenceGraphQueryService competenceGraph,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var results = await competenceGraph.GetSkillExpertsAsync(query.SkillId, query.Limit, cancellationToken)
            .ConfigureAwait(false);

        return results.Select(r => new SkillExpertView(
            r.ParticipantId.Value,
            r.EndorsementCount,
            r.AverageGrade,
            r.TotalScore)).ToList();
    }
}
