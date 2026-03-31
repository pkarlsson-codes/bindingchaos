using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Signals;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Withdraw amplification command.
/// </summary>
/// <param name="ActorId">Id of the actor withdrawing their amplification.</param>
/// <param name="SignalId">Id of the signal to withdraw amplification from.</param>
public sealed record WithdrawAmplification(ParticipantId ActorId, SignalId SignalId);

/// <summary>
/// A <see cref="WithdrawAmplification"/> command handler.
/// </summary>
public static class WithdrawAmplificationHandler
{
    /// <summary>
    /// Handles a <see cref="WithdrawAmplification"/> command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="signalRepository">A signal repository.</param>
    /// <param name="unitOfWork">A unit of work.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        WithdrawAmplification command,
        ISignalRepository signalRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var signal = await signalRepository
            .GetByIdOrThrowAsync(command.SignalId, cancellationToken)
            .ConfigureAwait(false);

        signal.WithdrawAmplification(command.ActorId);

        signalRepository.Stage(signal);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
