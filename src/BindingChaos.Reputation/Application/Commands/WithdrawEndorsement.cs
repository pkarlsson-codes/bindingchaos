using BindingChaos.Reputation.Domain.SkillEndorsements;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to remove an endorsement. Idempotent — no-op if the endorsement does not exist.
/// </summary>
/// <param name="EndorserId">The participant who gave the endorsement.</param>
/// <param name="EndorseeId">The participant who was endorsed.</param>
/// <param name="SkillId">The skill.</param>
public sealed record WithdrawEndorsement(ParticipantId EndorserId, ParticipantId EndorseeId, Guid SkillId);

/// <summary>
/// Handles the <see cref="WithdrawEndorsement"/> command.
/// </summary>
public static class WithdrawEndorsementHandler
{
    /// <summary>
    /// Removes a skill endorsement. Idempotent — no-op if the endorsement does not exist.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The skill endorsement repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        WithdrawEndorsement command,
        ISkillEndorsementRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        await repository.WithdrawAsync(command.EndorserId, command.EndorseeId, command.SkillId, cancellationToken)
            .ConfigureAwait(false);
    }
}
