namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Represents a comment and its display data for the gateway.
/// </summary>
public sealed class CommentViewModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the comment.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the raw textual content of the comment.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the author who created the comment.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author's pseudonym for context-scoped display.
    /// </summary>
    public string AuthorPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the comment was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the comment was last updated; otherwise null.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent comment if this is a reply; otherwise null.
    /// </summary>
    public string? ParentCommentId { get; set; }

    /// <summary>
    /// Gets or sets the replies to this comment, if they were requested.
    /// </summary>
    public IReadOnlyCollection<CommentViewModel>? Replies { get; set; }

    /// <summary>
    /// Gets or sets the total number of replies to this comment.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the comment has been edited since creation.
    /// </summary>
    public bool IsEdited { get; set; }

    /// <summary>
    /// Gets or sets the type of the entity the comment belongs to (for example, "Signal").
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the entity the comment belongs to.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;
}

/// <summary>
/// Request payload to create a new comment or reply.
/// </summary>
public sealed class CreateCommentRequest
{
    /// <summary>
    /// Gets or sets the content of the comment to create.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent comment identifier when creating a reply; otherwise null.
    /// </summary>
    public string? ParentCommentId { get; set; }

    /// <summary>
    /// Gets or sets the type of the entity the comment will be associated with.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the entity the comment will be associated with.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;
}
