using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Domain.TrustRelationships;

/// <summary>
/// Read-side service for degree-scoped trust graph traversal.
/// Kept separate from the write repository so other bounded contexts inject only this interface.
/// </summary>
public interface ITrustGraphQueryService
{
    /// <summary>
    /// Returns the set of participant IDs reachable from <paramref name="participantId"/>
    /// within <paramref name="maxDegree"/> trust hops. Returns an empty set if no connections exist.
    /// </summary>
    /// <param name="participantId">The starting participant for the traversal.</param>
    /// <param name="maxDegree">The maximum number of trust hops to traverse. Clamped to [1, 5] by the implementation.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The set of participant IDs reachable within the specified degree.</returns>
    Task<IReadOnlySet<ParticipantId>> GetTrustedParticipantsAsync(
        ParticipantId participantId, int maxDegree, CancellationToken ct);

    /// <summary>
    /// Filters <paramref name="candidateIds"/> to those reachable from <paramref name="participantId"/>
    /// within <paramref name="maxDegree"/> trust hops. More efficient than <see cref="GetTrustedParticipantsAsync"/>
    /// when you only need to check a known set of participants.
    /// </summary>
    /// <param name="participantId">The starting participant for the traversal.</param>
    /// <param name="maxDegree">The maximum number of trust hops to traverse. Clamped to [1, 5] by the implementation.</param>
    /// <param name="candidateIds">The participant IDs to filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The subset of <paramref name="candidateIds"/> that are trusted within the specified degree.</returns>
    Task<IReadOnlySet<ParticipantId>> FilterTrustedParticipantsAsync(
        ParticipantId participantId, int maxDegree, IEnumerable<ParticipantId> candidateIds, CancellationToken ct);
}
