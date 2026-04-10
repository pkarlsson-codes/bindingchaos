using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Domain.SkillEndorsements;

#pragma warning disable SA1313 // Primary constructor parameters act as PascalCase properties
/// <summary>Result entry for skill expert queries.</summary>
/// <param name="ParticipantId">The expert's participant ID.</param>
/// <param name="EndorsementCount">Total number of endorsements received.</param>
/// <param name="AverageGrade">Mean grade across all endorsements.</param>
/// <param name="TotalScore">Sum of all grades (used for ranking).</param>
public sealed record SkillExpertResult(
    ParticipantId ParticipantId,
    int EndorsementCount,
    double AverageGrade,
    int TotalScore);

/// <summary>Result entry for a single competency in a participant's profile.</summary>
/// <param name="SkillId">The skill ID.</param>
/// <param name="EndorsementCount">Total number of endorsements received.</param>
/// <param name="AverageGrade">Mean grade across all endorsements.</param>
public sealed record CompetencyResult(Guid SkillId, int EndorsementCount, double AverageGrade);
#pragma warning restore SA1313

/// <summary>
/// Read-side service for competence graph queries.
/// Kept separate from the write repository so other bounded contexts can inject only this interface.
/// </summary>
public interface ICompetenceGraphQueryService
{
    /// <summary>
    /// Returns participants endorsed for <paramref name="skillId"/>, ranked by total endorsement score
    /// (sum of grades), descending.
    /// </summary>
    /// <param name="skillId">The skill to query experts for.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The ranked list of experts.</returns>
    Task<IReadOnlyList<SkillExpertResult>> GetSkillExpertsAsync(Guid skillId, int limit, CancellationToken ct);

    /// <summary>
    /// Returns the skills a participant has been endorsed for, together with endorsement counts
    /// and average grades, ordered by endorsement count descending.
    /// </summary>
    /// <param name="participantId">The participant to query.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The participant's competency profile.</returns>
    Task<IReadOnlyList<CompetencyResult>> GetParticipantCompetenciesAsync(
        ParticipantId participantId, CancellationToken ct);

    /// <summary>
    /// Returns participants endorsed for <paramref name="skillId"/> by participants within
    /// <paramref name="viewerId"/>'s trust graph (up to 3 hops), ranked by weighted score.
    /// This cross-cuts the trust graph to surface experts vouched for by trusted peers.
    /// </summary>
    /// <param name="viewerId">The participant whose trust graph is used for weighting.</param>
    /// <param name="skillId">The skill to query experts for.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The trust-weighted ranked list of experts.</returns>
    Task<IReadOnlyList<SkillExpertResult>> GetTrustWeightedExpertsAsync(
        ParticipantId viewerId, Guid skillId, int limit, CancellationToken ct);
}
