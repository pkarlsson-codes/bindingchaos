using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Signals;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Amplify signal command.
/// </summary>
/// <param name="ActorId">Id of the actor amplifying the signal.</param>
/// <param name="SignalId">Id of the signal to amplify.</param>
public sealed record AmplifySignal(ParticipantId ActorId, SignalId SignalId);

/// <summary>
/// An <see cref="AmplifySignal"/> command handler.
/// </summary>
public static class AmplifySignalHandler
{
    /// <summary>
    /// Handles an <see cref="AmplifySignal"/> command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="signalRepository">A signal repository.</param>
    /// <param name="unitOfWork">A unit of work.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        AmplifySignal command,
        ISignalRepository signalRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var signal = await signalRepository
            .GetByIdOrThrowAsync(command.SignalId, cancellationToken)
            .ConfigureAwait(false);

        signal.Amplify(command.ActorId);

        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
