using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Societies.Domain.Societies;

namespace BindingChaos.Societies.Application.Commands;

/// <summary>
/// Command for a participant to leave a society.
/// </summary>
/// <param name="SocietyId">The society to leave.</param>
/// <param name="ParticipantId">The participant leaving the society.</param>
public sealed record LeaveSociety(
    SocietyId SocietyId,
    ParticipantId ParticipantId);

/// <summary>
/// Handles the <see cref="LeaveSociety"/> command.
/// </summary>
public static class LeaveSocietyHandler
{
    /// <summary>
    /// Handles a <see cref="LeaveSociety"/> command, removing the participant's active membership.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="societyRepository">Repository for loading and staging the society.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        LeaveSociety command,
        ISocietyRepository societyRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var society = await societyRepository
            .GetByIdOrThrowAsync(command.SocietyId, cancellationToken)
            .ConfigureAwait(false);

        society.Leave(command.ParticipantId);

        societyRepository.Stage(society);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
