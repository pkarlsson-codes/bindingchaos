using System.ComponentModel.DataAnnotations;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Detailed view model for a signal including amplification history and metadata.
/// </summary>
internal sealed class SignalDetailViewModel
{
    /// <summary>
    /// The unique identifier of the signal.
    /// </summary>
    [Required]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The title of the signal.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the signal.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The tags associated with the signal.
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// The pseudonym of the author.
    /// </summary>
    public string AuthorPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// The creation timestamp in ISO 8601 format.
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// The last amplification timestamp in ISO 8601 format.
    /// </summary>
    public string? LastAmplifiedAt { get; set; }

    /// <summary>
    /// The total number of amplifications for this signal.
    /// </summary>
    public int AmplifyCount { get; set; }

    /// <summary>
    /// The list of amplifications for this signal.
    /// </summary>
    public AmplificationViewModel[] Amplifications { get; set; } = [];

    /// <summary>
    /// Indicates whether the current user has amplified this signal.
    /// </summary>
    public bool IsAmplifiedByCurrentUser { get; set; }

    /// <summary>
    /// Indicates whether the current user is the originator of this signal.
    /// </summary>
    public bool IsOriginator { get; set; }

    /// <summary>
    /// The attachments associated with this signal.
    /// </summary>
    public AttachmentDetailViewModel[] Attachments { get; set; } = [];

    /// <summary>
    /// The suggested actions for this signal.
    /// </summary>
    public SuggestedActionViewModel[] SuggestedActions { get; set; } = [];
}

/// <summary>
/// View model for signal amplifications.
/// </summary>
internal sealed class AmplificationViewModel
{
    /// <summary>
    /// The unique identifier of the amplification.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The pseudonym of the person who amplified the signal.
    /// </summary>
    public string AmplifierPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// The amplification timestamp in ISO 8601 format.
    /// </summary>
    public string AmplifiedAt { get; set; } = string.Empty;

    /// <summary>
    /// Optional comment provided with the amplification.
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// View model for a suggested action on a signal.
/// </summary>
internal sealed class SuggestedActionViewModel
{
    /// <summary>
    /// The unique identifier of the suggested action.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The action type code name (e.g. <c>MakeACall</c>, <c>VisitAWebpage</c>).
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// The phone number to call. Present for <c>MakeACall</c> actions.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The URL to visit. Present for <c>VisitAWebpage</c> actions.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Optional free-text context provided by the suggester.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// The pseudonym of the participant who suggested the action.
    /// </summary>
    public string SuggestedByPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// When the action was suggested, in ISO 8601 format.
    /// </summary>
    public string SuggestedAt { get; set; } = string.Empty;
}

/// <summary>
/// View model for signal attachment details including pre-built URLs for display.
/// </summary>
internal sealed class AttachmentDetailViewModel
{
    /// <summary>
    /// The unique identifier of the document in the document service.
    /// </summary>
    public string DocumentId { get; set; } = string.Empty;

    /// <summary>
    /// Optional caption for the attachment.
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// The URL to the thumbnail version of the attachment.
    /// </summary>
    public string ThumbnailUrl { get; set; } = string.Empty;

    /// <summary>
    /// The URL to the display-sized version of the attachment.
    /// </summary>
    public string DisplayUrl { get; set; } = string.Empty;
}
