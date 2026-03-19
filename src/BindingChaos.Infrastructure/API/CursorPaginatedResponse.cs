namespace BindingChaos.Infrastructure.API;

/// <summary>
/// Cursor-based paginated response wrapper for API endpoints.
/// Provides stable pagination using opaque cursors instead of offset-based pagination.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public sealed class CursorPaginatedResponse<T>
{
    /// <summary>
    /// The items for the current page.
    /// </summary>
    public T[] Items { get; set; } = [];

    /// <summary>
    /// Opaque cursor for retrieving the next page of results.
    /// Null if no more pages are available.
    /// </summary>
    public string? NextCursor { get; set; }

    /// <summary>
    /// Opaque cursor for retrieving the previous page of results.
    /// Null if this is the first page.
    /// </summary>
    public string? PreviousCursor { get; set; }

    /// <summary>
    /// Indicates whether there is a next page of results.
    /// </summary>
    public bool HasNextPage => !string.IsNullOrEmpty(NextCursor);

    /// <summary>
    /// Indicates whether there is a previous page of results.
    /// </summary>
    public bool HasPreviousPage => !string.IsNullOrEmpty(PreviousCursor);

    /// <summary>
    /// The number of items requested for this page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The request ID for tracking.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// The timestamp when the response was generated.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}