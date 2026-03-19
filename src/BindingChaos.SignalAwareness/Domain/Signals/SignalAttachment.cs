using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Represents an attachment associated with a signal, identified by a unique string key.
/// </summary>
internal sealed class SignalAttachment : Entity<SignalAttachmentId>
{
    private SignalAttachment(SignalAttachmentId id, string documentId, string? caption, DateTimeOffset createdAt)
    {
        Id = id;
        DocumentId = documentId;
        Caption = caption;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Gets the caption text associated with the object.
    /// </summary>
    public string? Caption { get; private set; }

    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets the unique identifier for the document.
    /// </summary>
    public string DocumentId { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="SignalAttachment"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier for the signal attachment.</param>
    /// <param name="documentId">The unique identifier of the document associated with the attachment. This value cannot be null.</param>
    /// <param name="caption">An optional caption describing the attachment. This value can be null.</param>
    /// <param name="createdAt">The date and time when the attachment was created.</param>
    /// <returns>A new <see cref="SignalAttachment"/> instance initialized with the specified identifier.</returns>
    public static SignalAttachment Create(SignalAttachmentId id, string documentId, string? caption, DateTimeOffset createdAt)
        => new(id, documentId, caption, createdAt);
}
