using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Application.DTOs;
using BindingChaos.SignalAwareness.Domain.Signals;

namespace BindingChaos.SignalAwareness.Application.Commands;

/// <summary>
/// Represents a signal captured within the system, including its metadata and associated details.
/// </summary>
/// <param name="Title">The title of the signal. This value cannot be null or empty.</param>
/// <param name="Description">A detailed description of the signal. This value cannot be null or empty.</param>
/// <param name="OriginatorId">The unique identifier of the participant who originated the signal.</param>
/// <param name="Location">The optional geographic location where this signal occurred.</param>
/// <param name="Tags">A collection of tags associated with the signal. This collection may be empty but cannot be null.</param>
/// <param name="Attachments">A collection of attachments related to the signal. This collection may be empty but cannot be null.</param>
public sealed record CaptureSignal(
    string Title,
    string Description,
    ParticipantId OriginatorId,
    Coordinates? Location,
    string[] Tags,
    AttachmentDto[] Attachments);

/// <summary>Handles the <see cref="CaptureSignal"/> command.</summary>
public static class CaptureSignalCommandHandler
{
    /// <summary>Captures a new signal and returns its identifier.</summary>
    /// <param name="command">The capture signal command.</param>
    /// <param name="signalRepository">Repository for staging the new signal.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The identifier of the newly captured signal.</returns>
    public static async Task<SignalId> Handle(
        CaptureSignal command,
        ISignalRepository signalRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var content = SignalContent.Create(command.Title, command.Description);
        AttachmentSpec[] attachmentEntities = [.. command.Attachments.Select(a => new AttachmentSpec(a.DocumentId, a.Caption))];
        var signal = Signal.Capture(content, command.OriginatorId, command.Location, command.Tags, attachmentEntities);
        signalRepository.Stage(signal);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return signal.Id;
    }
}
