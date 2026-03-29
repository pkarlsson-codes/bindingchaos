namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for signal list items.
/// </summary>
/// <param name="SignalId">The ID of the signal.</param>
/// <param name="Title">The title of the signal.</param>
/// <param name="Description">The description of the signal.</param>
/// <param name="OriginatorPseudonym">The pseudonym of the participant who captured the signal.</param>
/// <param name="CapturedAt">When the signal was captured.</param>
/// <param name="Latitude">Optional latitude of the signal's location.</param>
/// <param name="Longitude">Optional longitude of the signal's location.</param>
/// <param name="AmplificationCount">The number of active amplifications.</param>
/// <param name="Tags">The tags associated with this signal.</param>
/// <param name="IsAmplifiedByCurrentUser">Whether the current user has amplified this signal.</param>
/// <param name="IsOriginator">Whether the current user is the originator of this signal.</param>
/// <param name="AttachmentIds">The document IDs of attachments associated with this signal.</param>
public record SignalListItemResponse(
    string SignalId,
    string Title,
    string Description,
    string? OriginatorPseudonym,
    DateTimeOffset CapturedAt,
    double? Latitude,
    double? Longitude,
    int AmplificationCount,
    string[] Tags,
    bool IsAmplifiedByCurrentUser,
    bool IsOriginator,
    string[] AttachmentIds);
