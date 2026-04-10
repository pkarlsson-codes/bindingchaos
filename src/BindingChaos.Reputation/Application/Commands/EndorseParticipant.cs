using BindingChaos.Reputation.Domain.SkillEndorsements;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to endorse a participant for a skill at a given grade.
/// Idempotent — if an endorsement already exists for the same
/// (EndorserId, EndorseeId, SkillId), the grade is updated.
/// </summary>
/// <param name="EndorserId">The participant giving the endorsement.</param>
/// <param name="EndorseeId">The participant being endorsed.</param>
/// <param name="SkillId">The skill being endorsed.</param>
/// <param name="Grade">The grade assigned to the endorsement.</param>
public sealed record EndorseParticipant(
    ParticipantId EndorserId,
    ParticipantId EndorseeId,
    Guid SkillId,
    EndorsementGrade Grade);

/// <summary>
/// Handles the <see cref="EndorseParticipant"/> command.
/// </summary>
public static class EndorseParticipantHandler
{
    /// <summary>
    /// Creates or updates a skill endorsement. Idempotent — updates the grade if the
    /// endorsement already exists.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The skill endorsement repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        EndorseParticipant command,
        ISkillEndorsementRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var endorsement = SkillEndorsement.Create(command.EndorserId, command.EndorseeId, command.SkillId, command.Grade);
        await repository.EndorseAsync(endorsement, cancellationToken).ConfigureAwait(false);
    }
}
