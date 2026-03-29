namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for detailed signal information.
/// </summary>
/// <param name="SignalId">The ID of the signal.</param>
/// <param name="Title">The title of the signal.</param>
/// <param name="Description">The description of the signal.</param>
/// <param name="OriginatorPseudonym">The pseudonym of the signal originator.</param>
/// <param name="CapturedAt">When the signal was captured.</param>
/// <param name="LastAmplifiedAt">When the signal was last amplified, if ever.</param>
/// <param name="Latitude">Optional latitude of the signal's location.</param>
/// <param name="Longitude">Optional longitude of the signal's location.</param>
/// <param name="Amplifications">The list of amplifications for this signal.</param>
/// <param name="IsAmplifiedByCurrentUser">Whether the current user has amplified this signal.</param>
/// <param name="IsOriginator">Whether the current user is the originator of this signal.</param>
/// <param name="AttachmentIds">The document IDs of attachments associated with this signal.</param>
public record SignalResponse(
    string SignalId,
    string Title,
    string Description,
    string OriginatorPseudonym,
    DateTimeOffset CapturedAt,
    DateTimeOffset? LastAmplifiedAt,
    double? Latitude,
    double? Longitude,
    IReadOnlyCollection<SignalResponse.Amplification> Amplifications,
    bool IsAmplifiedByCurrentUser,
    bool IsOriginator,
    IReadOnlyCollection<string> AttachmentIds)
{
    /// <summary>
    /// Response model for signal amplification with pseudonym privacy protection.
    /// </summary>
    /// <param name="AmplifierPseudonym">The pseudonym of the participant who amplified the signal.</param>
    /// <param name="AmplifiedAt">When the amplification occurred.</param>
    public record Amplification(
        string AmplifierPseudonym,
        DateTimeOffset AmplifiedAt);
}
