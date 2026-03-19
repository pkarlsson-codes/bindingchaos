namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for signals tailored to the web frontend.
/// </summary>
public sealed record SignalViewModel
{
    /// <summary>
    /// The unique identifier of the signal.
    /// </summary>
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
    /// The number of amplifications for this signal.
    /// </summary>
    public int AmplificationCount { get; set; }

    /// <summary>
    /// The number of comments for this signal.
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Indicates whether the current user has amplified this signal.
    /// </summary>
    public bool IsAmplifiedByCurrentUser { get; set; }

    /// <summary>
    /// Indicates whether the current user is the originator of this signal.
    /// </summary>
    public bool IsOriginator { get; set; }

    /// <summary>
    /// URL to the first attachment thumbnail (if any images exist).
    /// This provides a visual preview for the signal in the list view.
    /// </summary>
    public string? FirstAttachmentThumbnail { get; set; }

    /// <summary>
    /// Total number of attachments associated with this signal.
    /// </summary>
    public int AttachmentCount { get; set; }
}