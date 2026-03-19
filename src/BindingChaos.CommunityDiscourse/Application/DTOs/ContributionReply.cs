namespace BindingChaos.CommunityDiscourse.Application.DTOs;

/// <summary>
/// Individual reply in the response.
/// </summary>
public class ContributionReply
{
    /// <summary>
    /// The unique identifier for this reply.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the participant who authored this reply.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// The pseudonym of the author.
    /// </summary>
    public string AuthorPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// The content of the reply.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// When the reply was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The ID of the contribution this reply responds to.
    /// </summary>
    public string? ParentContributionId { get; set; }

    /// <summary>
    /// Always empty for one-level loading (API compatibility).
    /// </summary>
    public List<ContributionReply> Replies { get; set; } = new();

    /// <summary>
    /// The total number of nested replies.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Whether there are nested replies.
    /// </summary>
    public bool HasMoreReplies { get; set; }
}
