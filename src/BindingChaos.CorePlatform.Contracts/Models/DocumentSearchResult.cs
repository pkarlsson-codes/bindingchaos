namespace BindingChaos.CorePlatform.Contracts.Models;

/// <summary>
/// Represents the result of a document search operation.
/// </summary>
public sealed class DocumentSearchResult
{
    /// <summary>
    /// Initializes a new instance of the DocumentSearchResult class.
    /// </summary>
    /// <param name="documents">The documents matching the search criteria.</param>
    /// <param name="totalCount">The total number of documents matching the criteria.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of documents per page.</param>
    public DocumentSearchResult(
        IEnumerable<DocumentMetadata> documents,
        long totalCount,
        int pageNumber,
        int pageSize)
    {
        Documents = documents.ToList();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Gets the documents matching the search criteria.
    /// </summary>
    public IReadOnlyList<DocumentMetadata> Documents { get; }

    /// <summary>
    /// Gets the total number of documents matching the search criteria.
    /// </summary>
    public long TotalCount { get; }

    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the number of documents per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets a value indicating whether there are more pages available.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets a value indicating whether there are previous pages available.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}