namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for signal information with pseudonym privacy protection.
/// </summary>
/// <param name="SignalId">The ID of the signal.</param>
/// <param name="Title">The title of the signal.</param>
/// <param name="Description">The description of the signal.</param>
/// <param name="AuthorId">The id of the author who created the signal.</param>
/// <param name="Status">The status of the signal.</param>
/// <param name="CapturedAt">When the signal was captured.</param>
/// <param name="Latitude">Optional latitude of the signal's location.</param>
/// <param name="Longitude">Optional longitude of the signal's location.</param>
/// <param name="AmplificationCount">The amplifications for this signal.</param>
/// <param name="Tags">The tags associated with this signal.</param>
/// <param name="IsAmplifiedByCurrentUser">Whether the current user has amplified this signal.</param>
/// <param name="IsOriginator">Whether the current user is the originator of this signal.</param>
/// <param name="Attachments">The attachments associated with this signal.</param>
public record SignalListItemResponse(
    string SignalId,
    string Title,
    string Description,
    string? AuthorId,
    string Status,
    DateTimeOffset CapturedAt,
    double? Latitude,
    double? Longitude,
    int AmplificationCount,
    string[] Tags,
    bool IsAmplifiedByCurrentUser,
    bool IsOriginator,
    AttachmentResponse[] Attachments);

/// <summary>
/// Response model for signal attachment information.
/// </summary>
/// <param name="Id">The unique identifier for the attachment.</param>
/// <param name="DocumentId">The document ID stored in the document service.</param>
/// <param name="Caption">Optional caption for the attachment.</param>
/// <param name="CreatedAt">When the attachment was added.</param>
public record AttachmentResponse(
    string Id,
    string DocumentId,
    string? Caption,
    DateTimeOffset CreatedAt);
