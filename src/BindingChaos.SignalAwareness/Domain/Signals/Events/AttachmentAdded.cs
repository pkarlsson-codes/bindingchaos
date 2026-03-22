using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Signals.Events;

/// <summary>
/// Represents an event indicating that an attachment has been added to a document.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate to which the attachment belongs.</param>
/// <param name="AttachmentId">The unique identifier of the added attachment.</param>
/// <param name="DocumentId">The unique identifier of the document associated with the attachment.</param>
/// <param name="Caption">An optional caption or description for the attachment. Can be <see langword="null"/> if no caption is provided.</param>
public sealed record AttachmentAdded(
    string AggregateId,
    string AttachmentId,
    string DocumentId,
    string? Caption
) : DomainEvent(AggregateId);
