using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Concerns;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Withdraws a participant's affectedness declaration for a concern.</summary>
/// <param name="ConcernId">The ID of the concern.</param>
/// <param name="ParticipantId">The ID of the participant withdrawing affectedness.</param>
public sealed record WithdrawAffectedness(ConcernId ConcernId, ParticipantId ParticipantId);

/// <summary>Handles the <see cref="WithdrawAffectedness"/> command.</summary>
public static class WithdrawAffectednessHandler
{
    /// <summary>Handles the <see cref="WithdrawAffectedness"/> command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="concernRepository">The concern repository.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Handle(
        WithdrawAffectedness command,
        IConcernRepository concernRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var concern = await concernRepository
            .GetByIdOrThrowAsync(command.ConcernId, cancellationToken)
            .ConfigureAwait(false);

        concern.WithdrawAffectedness(command.ParticipantId);
        concernRepository.Stage(concern);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
