namespace BindingChaos.CommunityDiscourse.Application.DTOs;

/// <summary>
/// Individual root contribution in the response.
/// </summary>
public class RootContribution
{
    /// <summary>
    /// The unique identifier for this contribution.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the participant who authored this contribution.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// The pseudonym of the author.
    /// </summary>
    public string AuthorPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// The content of the contribution.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// When the contribution was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The total number of direct replies.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// When the last reply was posted.
    /// </summary>
    public DateTimeOffset? LastReplyAt { get; set; }

    /// <summary>
    /// Whether this contribution has replies.
    /// </summary>
    public bool HasReplies => ReplyCount > 0;
}
