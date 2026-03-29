using System.Reflection.Metadata;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Signals;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Capture signal command.
/// </summary>
/// <param name="ActorId">Id of the actor capturing the signal.</param>
/// <param name="Description">Description of the signal.</param>
/// <param name="Tags">Tags assigned to the signal.</param>
public record CaptureSignal(
    ParticipantId ActorId,
    string Description,
    IReadOnlyList<string> Tags);

/// <summary>
/// A <see cref="CaptureSignal"/> command handler.
/// </summary>
public static class CaptureSignalHandler
{
    /// <summary>
    /// Handles a <see cref="CaptureSignal"/> command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="signalRepository">A signal repository.</param>
    /// <param name="unitOfWork">A unit of work.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Id of the captured signal.</returns>
    public static async Task<SignalId> Handle(
        CaptureSignal command,
        ISignalRepository signalRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var signal = Signal.Capture(command.ActorId, command.Description, command.Tags);
        signalRepository.Stage(signal);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return signal.Id;
    }
}