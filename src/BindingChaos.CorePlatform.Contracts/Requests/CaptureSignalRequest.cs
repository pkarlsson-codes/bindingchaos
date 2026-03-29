namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Capture signal request model.
/// </summary>
/// <param name="Title">The title of the signal.</param>
/// <param name="Description">The description of the signal.</param>
/// <param name="Tags">The tags for the signal.</param>
/// <param name="AttachmentIds">The document IDs of attachments to associate with the signal.</param>
/// <param name="Latitude">Optional latitude coordinate of the signal's location.</param>
/// <param name="Longitude">Optional longitude coordinate of the signal's location.</param>
public record CaptureSignalRequest(
    string Title,
    string Description,
    string[] Tags,
    string[] AttachmentIds,
    double? Latitude,
    double? Longitude);
