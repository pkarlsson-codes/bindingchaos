namespace BindingChaos.CommunityDiscourse.Application.ReadModels;

/// <summary>
/// Contribution document for efficient querying of contributions and replies.
/// </summary>
public class ContributionView
{
    /// <summary>
    /// Gets or sets the unique identifier for the contribution.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the thread this contribution belongs to.
    /// </summary>
    public string ThreadId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the participant who authored this contribution.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the contribution.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the contribution was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent contribution, if this is a reply.
    /// </summary>
    public string? ParentContributionId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this contribution is a root-level contribution (not a reply).
    /// </summary>
    public bool IsRootContribution { get; set; }

    /// <summary>
    /// Gets or sets the number of replies to this contribution.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this contribution last received a reply.
    /// </summary>
    public DateTimeOffset? LastReplyAt { get; set; }
}
