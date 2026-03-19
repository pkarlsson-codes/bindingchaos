using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Domain.Signals;

namespace BindingChaos.SignalAwareness.Application.Commands;

/// <summary>
/// Represents a request to deamplify a signal that was previously amplified by a specific participant.
/// </summary>
/// <param name="SignalId">The unique identifier of the signal to be deamplified.</param>
/// <param name="AmplifierId">The unique identifier of the participant who amplified the signal.</param>
public sealed record DeamplifySignal(SignalId SignalId, ParticipantId AmplifierId);

/// <summary>Handles the <see cref="DeamplifySignal"/> command.</summary>
public static class DeamplifySignalCommandHandler
{
    /// <summary>Deamplifies a signal and returns the updated active amplification count.</summary>
    /// <param name="command">The deamplify signal command.</param>
    /// <param name="signalRepository">Repository for loading and staging the signal.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated active amplification count.</returns>
    public static async Task<int> Handle(
        DeamplifySignal command,
        ISignalRepository signalRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var signal = await signalRepository.GetByIdAsync(command.SignalId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Signal with ID {command.SignalId.Value} not found");
        signal.Attenuate(command.AmplifierId);
        signalRepository.Stage(signal);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return signal.ActiveAmplifications.Count;
    }
}
