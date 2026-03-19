namespace BindingChaos.CorePlatform.Contracts.Models;

/// <summary>
/// Represents search criteria for finding documents.
/// </summary>
public sealed class DocumentSearchCriteria
{
    /// <summary>
    /// Initializes a new instance of the DocumentSearchCriteria class.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <param name="contentTypes">Filter by specific content types.</param>
    /// <param name="createdAfter">Filter documents created after this date.</param>
    /// <param name="createdBefore">Filter documents created before this date.</param>
    /// <param name="createdBy">Filter documents created by a specific user.</param>
    /// <param name="properties">Filter by custom properties.</param>
    /// <param name="pageNumber">The page number for pagination (1-based).</param>
    /// <param name="pageSize">The number of documents per page.</param>
    /// <param name="sortBy">The field to sort by.</param>
    /// <param name="sortDirection">The sort direction.</param>
    public DocumentSearchCriteria(
        string? query = null,
        IEnumerable<string>? contentTypes = null,
        DateTimeOffset? createdAfter = null,
        DateTimeOffset? createdBefore = null,
        string? createdBy = null,
        Dictionary<string, object>? properties = null,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        SortDirection sortDirection = SortDirection.Descending)
    {
        Query = query;
        ContentTypes = contentTypes?.ToList() ?? new List<string>();
        CreatedAfter = createdAfter;
        CreatedBefore = createdBefore;
        CreatedBy = createdBy;
        Properties = properties ?? new Dictionary<string, object>();
        PageNumber = Math.Max(1, pageNumber);
        PageSize = Math.Max(1, Math.Min(100, pageSize)); // Limit to 100 items per page
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Gets the search query string for full-text search.
    /// </summary>
    public string? Query { get; }

    /// <summary>
    /// Gets the content types to filter by.
    /// </summary>
    public IReadOnlyList<string> ContentTypes { get; }

    /// <summary>
    /// Gets the date to filter documents created after.
    /// </summary>
    public DateTimeOffset? CreatedAfter { get; }

    /// <summary>
    /// Gets the date to filter documents created before.
    /// </summary>
    public DateTimeOffset? CreatedBefore { get; }

    /// <summary>
    /// Gets the user to filter documents created by.
    /// </summary>
    public string? CreatedBy { get; }

    /// <summary>
    /// Gets custom properties to filter by.
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets the page number for pagination (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the number of documents per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the field to sort by.
    /// </summary>
    public string? SortBy { get; }

    /// <summary>
    /// Gets the sort direction.
    /// </summary>
    public SortDirection SortDirection { get; }

    /// <summary>
    /// Gets the number of documents to skip for pagination.
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;
}

/// <summary>
/// Represents the direction for sorting.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Sort in ascending order.
    /// </summary>
    Ascending,

    /// <summary>
    /// Sort in descending order.
    /// </summary>
    Descending,
}