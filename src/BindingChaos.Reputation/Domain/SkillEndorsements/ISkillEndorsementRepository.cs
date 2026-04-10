using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Domain.SkillEndorsements;

/// <summary>
/// Write-side repository for skill endorsements.
/// </summary>
public interface ISkillEndorsementRepository
{
    /// <summary>
    /// Creates or updates the endorsement. Idempotent — if an endorsement already exists for
    /// (EndorserId, EndorseeId, SkillId), its grade is updated.
    /// </summary>
    /// <param name="endorsement">The endorsement to persist.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EndorseAsync(SkillEndorsement endorsement, CancellationToken ct);

    /// <summary>
    /// Updates the grade of an existing endorsement. No-op if the endorsement does not exist.
    /// </summary>
    /// <param name="endorserId">The endorsing participant.</param>
    /// <param name="endorseeId">The endorsed participant.</param>
    /// <param name="skillId">The skill.</param>
    /// <param name="newGrade">The new grade.</param>
    /// <param name="updatedAt">The revision timestamp.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ReviseAsync(
        ParticipantId endorserId,
        ParticipantId endorseeId,
        Guid skillId,
        EndorsementGrade newGrade,
        DateTimeOffset updatedAt,
        CancellationToken ct);

    /// <summary>Removes an endorsement. No-op if it does not exist.</summary>
    /// <param name="endorserId">The endorsing participant.</param>
    /// <param name="endorseeId">The endorsed participant.</param>
    /// <param name="skillId">The skill.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WithdrawAsync(ParticipantId endorserId, ParticipantId endorseeId, Guid skillId, CancellationToken ct);

    /// <summary>
    /// Returns <see langword="true"/> if an endorsement exists for the given composite identity.
    /// </summary>
    /// <param name="endorserId">The endorsing participant.</param>
    /// <param name="endorseeId">The endorsed participant.</param>
    /// <param name="skillId">The skill.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><see langword="true"/> if the endorsement exists; otherwise <see langword="false"/>.</returns>
    Task<bool> ExistsAsync(ParticipantId endorserId, ParticipantId endorseeId, Guid skillId, CancellationToken ct);
}
