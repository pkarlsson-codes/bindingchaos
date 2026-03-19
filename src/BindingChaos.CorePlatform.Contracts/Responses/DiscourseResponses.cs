using BindingChaos.Infrastructure.API;

namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for getting contributions for an entity.
/// </summary>
public sealed class GetContributionsResponse
{
    /// <summary>
    /// Gets or sets the type of entity this discourse is about.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the entity this discourse is about.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the thread ID for this discourse.
    /// </summary>
    public string ThreadId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of all contributions for this entity (includes replies).
    /// </summary>
    public int TotalContributionCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of root contributions for this entity.
    /// </summary>
    public int TotalRootContributionCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of unique participants.
    /// </summary>
    public int TotalParticipants { get; set; }

    /// <summary>
    /// Gets or sets when the last contribution was posted.
    /// </summary>
    public DateTimeOffset LastActivityAt { get; set; }

    /// <summary>
    /// Gets or sets the cursor-paginated root contributions for this entity.
    /// </summary>
    public CursorPaginatedResponse<ContributionResponse> Contributions { get; set; } = new();
}

/// <summary>
/// Response model for an individual contribution.
/// </summary>
public sealed class ContributionResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for this contribution.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the participant who authored this contribution.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the pseudonym of the author.
    /// </summary>
    public string AuthorPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the contribution.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the contribution was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the total number of direct replies.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Gets or sets when the last reply was posted.
    /// </summary>
    public DateTimeOffset? LastReplyAt { get; set; }

    /// <summary>
    /// Gets whether this contribution has replies.
    /// </summary>
    public bool HasReplies { get; set; }
}

/// <summary>
/// Response model for posting a contribution.
/// </summary>
/// <param name="ContributionId">The ID of the created contribution.</param>
public sealed record PostContributionResponse(string ContributionId);

/// <summary>
/// Response model for getting replies to a contribution.
/// </summary>
public sealed class GetContributionRepliesResponse
{
    /// <summary>
    /// Gets or sets the ID of the parent contribution these replies belong to.
    /// </summary>
    public string ContributionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cursor-paginated replies to this contribution.
    /// </summary>
    public CursorPaginatedResponse<ContributionReplyResponse> Replies { get; set; } = new();
}

/// <summary>
/// Response model for an individual reply.
/// </summary>
public sealed class ContributionReplyResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for this reply.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the participant who authored this reply.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the pseudonym of the author.
    /// </summary>
    public string AuthorPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the reply.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the reply was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the ID of the contribution this reply responds to.
    /// </summary>
    public string? ParentContributionId { get; set; }

    /// <summary>
    /// Gets or sets the total number of nested replies.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Gets or sets whether there are nested replies.
    /// </summary>
    public bool HasMoreReplies { get; set; }
}