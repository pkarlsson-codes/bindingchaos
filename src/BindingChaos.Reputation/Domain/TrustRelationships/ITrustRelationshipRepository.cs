using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Domain.TrustRelationships;

/// <summary>
/// Write-side repository for trust relationships.
/// </summary>
public interface ITrustRelationshipRepository
{
    /// <summary>Creates a trust relationship. Idempotent — no-op if the relationship already exists.</summary>
    /// <param name="relationship">The trust relationship to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task TrustAsync(TrustRelationship relationship, CancellationToken ct);

    /// <summary>Removes a trust relationship. No-op if it does not exist.</summary>
    /// <param name="trusterId">The ID of the participant who extended trust.</param>
    /// <param name="trusteeId">The ID of the participant who was trusted.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WithdrawAsync(ParticipantId trusterId, ParticipantId trusteeId, CancellationToken ct);

    /// <summary>
    /// Returns <see langword="true"/> if a direct trust relationship exists from
    /// <paramref name="trusterId"/> to <paramref name="trusteeId"/>.
    /// </summary>
    /// <param name="trusterId">The ID of the participant who extended trust.</param>
    /// <param name="trusteeId">The ID of the participant who was trusted.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><see langword="true"/> if the relationship exists; otherwise <see langword="false"/>.</returns>
    Task<bool> ExistsAsync(ParticipantId trusterId, ParticipantId trusteeId, CancellationToken ct);
}
