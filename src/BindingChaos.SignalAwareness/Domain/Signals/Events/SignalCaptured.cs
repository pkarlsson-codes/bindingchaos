using BindingChaos.SharedKernel.Domain.Events;
using static BindingChaos.SignalAwareness.Domain.Signals.Events.SignalCaptured;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Domain event emitted when a new signal is captured in the system.
/// </summary>
/// <param name="AggregateId">The unique identifier of the captured signal.</param>
/// <param name="Title">The title of the signal.</param>
/// <param name="Description">The description of the signal.</param>
/// <param name="OriginatorId">The identifier of the participant who captured the signal.</param>
/// <param name="Latitude">The optional latitude of the location where this signal occurred.</param>
/// <param name="Longitude">The optional longitude of the location where this signal occurred.</param>
/// <param name="Tags">Optional tags associated with the signal.</param>
/// <param name="Attachments">Optional attachments added to the signal.</param>
public sealed record SignalCaptured(
    string AggregateId,
    string Title,
    string Description,
    string OriginatorId,
    double? Latitude,
    double? Longitude,
    string[] Tags,
    AttachmentRecord[] Attachments
) : DomainEvent(AggregateId)
{
    /// <summary>
    /// Represents a record of an attachment, including its associated document identifier and an optional caption.
    /// </summary>
    /// <param name="AttachmentId">The unique identifier of the attachment.</param>
    /// <param name="DocumentId">The unique identifier of the document associated with the attachment. This value cannot be null.</param>
    /// <param name="Caption">An optional caption for the attachment, providing additional context or description.</param>
    public sealed record AttachmentRecord(string AttachmentId, string DocumentId, string? Caption);
}
