namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for creating a signal.
/// </summary>
/// <param name="Title">The title of the signal.</param>
/// <param name="Description">The description of the signal.</param>
/// <param name="Tags">The tags for the signal, comma-separated.</param>
/// <param name="Attachments">The attachments associated with the signal.</param>
/// <param name="Latitude">Optional latitude coordinate of the signal's location.</param>
/// <param name="Longitude">Optional longitude coordinate of the signal's location.</param>
public record CaptureSignalRequest(
    string Title,
    string Description,
    string[] Tags,
    CaptureSignalRequest.Attachment[] Attachments,
    double? Latitude,
    double? Longitude)
{
    /// <summary>
    /// Represents an attachment associated with a document, including its identifier and an optional caption.
    /// </summary>
    /// <param name="DocumentId">The unique identifier of the document associated with the attachment. This value cannot be null.</param>
    /// <param name="Caption">An optional caption describing the attachment. This value can be null.</param>
    public record Attachment(string DocumentId, string? Caption);
}
