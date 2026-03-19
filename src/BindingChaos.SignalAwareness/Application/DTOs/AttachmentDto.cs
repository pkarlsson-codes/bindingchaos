namespace BindingChaos.SignalAwareness.Application.DTOs;

/// <summary>
/// Represents an attachment associated with a signal, including its document identifier and an optional caption.
/// </summary>
/// <param name="DocumentId">The unique identifier of the document associated with the attachment. This value cannot be null.</param>
/// <param name="Caption">An optional caption describing the attachment. This value can be null.</param>
public record AttachmentDto(string DocumentId, string? Caption);
