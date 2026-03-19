using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Domain.Signals;

namespace BindingChaos.SignalAwareness.Application.Commands;

/// <summary>
/// Represents an amplification action performed on a signal by a participant, including the reason and optional
/// commentary.
/// </summary>
/// <param name="SignalId">The unique identifier of the signal being amplified.</param>
/// <param name="AmplifierId">The unique identifier of the participant performing the amplification.</param>
/// <param name="Reason">The reason for amplifying the signal.</param>
/// <param name="Commentary">Optional commentary or additional context provided for the amplification. Can be <see langword="null"/>.</param>
public sealed record AmplifySignal(SignalId SignalId, ParticipantId AmplifierId, AmplificationReason Reason, string? Commentary);

/// <summary>Handles the <see cref="AmplifySignal"/> command.</summary>
public static class AmplifySignalHandler
{
    /// <summary>Amplifies a signal and returns the updated active amplification count.</summary>
    /// <param name="request">The amplify signal command.</param>
    /// <param name="signalRepository">Repository for loading and staging the signal.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated active amplification count.</returns>
    public static async Task<int> Handle(
        AmplifySignal request,
        ISignalRepository signalRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var signal = await signalRepository.GetByIdAsync(request.SignalId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Signal with ID {request.SignalId.Value} not found");
        signal.Amplify(request.AmplifierId, request.Reason, request.Commentary);
        signalRepository.Stage(signal);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return signal.ActiveAmplifications.Count;
    }
}
