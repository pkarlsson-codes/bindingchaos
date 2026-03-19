using static BindingChaos.CorePlatform.Contracts.Responses.SignalResponse;

namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for detailed signal information.
/// </summary>
/// <param name="SignalId">The ID of the signal.</param>
/// <param name="Title">The title of the signal.</param>
/// <param name="Description">The description of the signal.</param>
/// <param name="OriginatorPseudonym">The pseudonym of the signal originator.</param>
/// <param name="Status">The status of the signal.</param>
/// <param name="CapturedAt">When the signal was captured.</param>
/// <param name="Latitude">Optional latitude of the signal's location.</param>
/// <param name="Longitude">Optional longitude of the signal's location.</param>
/// <param name="Amplifications">The list of amplifications for this signal.</param>
/// <param name="IsAmplifiedByCurrentUser">Whether the current user has amplified this signal.</param>
/// <param name="IsOriginator">Whether the current user is the originator of this signal.</param>
/// <param name="Attachments">The attachments associated with this signal.</param>
/// <param name="SuggestedActions">The suggested actions for this signal.</param>
public record SignalResponse(
    string SignalId,
    string Title,
    string Description,
    string OriginatorPseudonym,
    string Status,
    DateTimeOffset CapturedAt,
    double? Latitude,
    double? Longitude,
    IReadOnlyCollection<Amplification> Amplifications,
    bool IsAmplifiedByCurrentUser,
    bool IsOriginator,
    IReadOnlyCollection<AttachmentResponse> Attachments,
    IReadOnlyCollection<SuggestedAction> SuggestedActions)
{
    /// <summary>
    /// Response model for signal amplification with pseudonym privacy protection.
    /// </summary>
    /// <param name="AmplificationId">The ID of the amplification.</param>
    /// <param name="AmplifierPseudonym">The pseudonym of the participant who amplified the signal.</param>
    /// <param name="Reason">The reason for the amplification.</param>
    /// <param name="Commentary">Optional commentary provided with the amplification.</param>
    /// <param name="AmplifiedAt">When the amplification occurred.</param>
    public record Amplification(
        string AmplificationId,
        string AmplifierPseudonym,
        string Reason,
        string? Commentary,
        DateTimeOffset AmplifiedAt);

    /// <summary>
    /// Response model for a structured suggested action on a signal.
    /// </summary>
    /// <param name="ActionId">The ID of the suggested action.</param>
    /// <param name="ActionType">The action type name (e.g. <c>MakeACall</c>, <c>VisitAWebpage</c>).</param>
    /// <param name="PhoneNumber">The phone number to call. Present for <c>MakeACall</c> actions.</param>
    /// <param name="Url">The URL to visit. Present for <c>VisitAWebpage</c> actions.</param>
    /// <param name="Details">Optional free-text context provided by the suggester.</param>
    /// <param name="SuggestedByPseudonym">The pseudonym of the participant who suggested the action.</param>
    /// <param name="SuggestedAt">When the action was suggested.</param>
    public record SuggestedAction(
        string ActionId,
        string ActionType,
        string? PhoneNumber,
        string? Url,
        string? Details,
        string SuggestedByPseudonym,
        DateTimeOffset SuggestedAt);
}
