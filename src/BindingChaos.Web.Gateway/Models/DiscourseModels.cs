using BindingChaos.Infrastructure.API;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Represents posts for a specific thread in the gateway with cursor-based pagination.
/// </summary>
public sealed class PostsViewModel
{
    /// <summary>
    /// Gets or sets the type of entity this thread is about.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the entity this thread is about.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the thread ID for this discourse.
    /// </summary>
    public string ThreadId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of all posts in this thread (includes replies).
    /// </summary>
    public int TotalPostCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of root posts in this thread.
    /// </summary>
    public int TotalRootPostCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of unique participants.
    /// </summary>
    public int TotalParticipants { get; set; }

    /// <summary>
    /// Gets or sets when the last post was created.
    /// </summary>
    public DateTimeOffset LastActivityAt { get; set; }

    /// <summary>
    /// Gets or sets the cursor-paginated root posts for this thread.
    /// </summary>
    public CursorPaginatedResponse<PostViewModel> Posts { get; set; } = new();
}

/// <summary>
/// Represents an individual post in the gateway.
/// </summary>
public sealed class PostViewModel
{
    /// <summary>
    /// Gets or sets the unique identifier for this post.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the participant who authored this post.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the pseudonym of the author.
    /// </summary>
    public string AuthorPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the post.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the post was created.
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
    /// Gets whether this post has replies.
    /// </summary>
    public bool HasReplies => ReplyCount > 0;
}

/// <summary>
/// Represents replies to a post with cursor-based pagination.
/// </summary>
public sealed class PostRepliesViewModel
{
    /// <summary>
    /// Gets or sets the ID of the parent post these replies belong to.
    /// </summary>
    public string PostId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cursor-paginated replies to this post.
    /// </summary>
    public CursorPaginatedResponse<ReplyViewModel> Replies { get; set; } = new();
}

/// <summary>
/// Represents an individual reply in the gateway.
/// </summary>
public sealed class ReplyViewModel
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
    /// Gets or sets the ID of the post this reply responds to.
    /// </summary>
    public string? ParentPostId { get; set; }

    /// <summary>
    /// Gets or sets the total number of nested replies.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Gets or sets whether there are nested replies.
    /// </summary>
    public bool HasMoreReplies { get; set; }
}

/// <summary>
/// Request payload to create a new post.
/// </summary>
public sealed class CreatePostRequest
{
    /// <summary>
    /// Gets or sets the content of the post.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Request payload to create a new reply.
/// </summary>
public sealed class CreateReplyRequest
{
    /// <summary>
    /// Gets or sets the content of the reply.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}