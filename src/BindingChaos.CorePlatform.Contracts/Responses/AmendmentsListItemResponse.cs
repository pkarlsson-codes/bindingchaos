namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Represents a response for an amendments list item.
/// </summary>
public sealed record AmendmentsListItemResponse
{
    /// <summary>
    /// Gets the amendment identifier.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the identifier of the idea this amendment belongs to.
    /// </summary>
    public string IdeaId { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the content was authored by the current user.
    /// </summary>
    public bool AuthoredByCurrentUser { get; init; }

    /// <summary>
    /// Gets the author identifier.
    /// </summary>
    public string AuthorPseudonym { get; init; } = string.Empty;

    /// <summary>
    /// Gets the amendment title.
    /// </summary>
    public string AmendmentTitle { get; init; } = string.Empty;

    /// <summary>
    /// Gets the amendment description.
    /// </summary>
    public string AmendmentDescription { get; init; } = string.Empty;

    /// <summary>
    /// Gets the status of the amendment.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the number of opponents for this amendment.
    /// </summary>
    public int OpponentCount { get; init; }

    /// <summary>
    /// Gets a value indicating whether the current user has opposed the item.
    /// </summary>
    public bool OpposedByCurrentUser { get; init; }

    /// <summary>
    /// Gets the number of supporters for this amendment.
    /// </summary>
    public int SupporterCount { get; init; }

    /// <summary>
    /// Gets the number of items or features supported by the current user.
    /// </summary>
    public bool SupportedByCurrentUser { get; init; }

    /// <summary>
    /// Gets when the amendment was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
