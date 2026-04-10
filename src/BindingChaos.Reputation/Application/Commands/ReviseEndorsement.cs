using BindingChaos.Reputation.Domain.SkillEndorsements;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to update the grade of an existing endorsement.
/// No-op if the endorsement does not exist.
/// </summary>
/// <param name="EndorserId">The participant who gave the endorsement.</param>
/// <param name="EndorseeId">The participant who was endorsed.</param>
/// <param name="SkillId">The skill.</param>
/// <param name="NewGrade">The updated grade.</param>
public sealed record ReviseEndorsement(
    ParticipantId EndorserId,
    ParticipantId EndorseeId,
    Guid SkillId,
    EndorsementGrade NewGrade);

/// <summary>
/// Handles the <see cref="ReviseEndorsement"/> command.
/// </summary>
public static class ReviseEndorsementHandler
{
    /// <summary>
    /// Updates the grade of an existing endorsement. No-op if the endorsement does not exist.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The skill endorsement repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ReviseEndorsement command,
        ISkillEndorsementRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var updatedAt = TimeProviderContext.Current.UtcNow;
        await repository.ReviseAsync(
            command.EndorserId,
            command.EndorseeId,
            command.SkillId,
            command.NewGrade,
            updatedAt,
            cancellationToken).ConfigureAwait(false);
    }
}
