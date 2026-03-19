namespace BindingChaos.Infrastructure.API;

/// <summary>
/// Extension methods for <see cref="PaginatedResponse{T}"/>.
/// </summary>
public static class PaginatedResponseExtensions
{
    /// <summary>
    /// Maps the items of a paginated response from one type to another using the specified mapping function.
    /// </summary>
    /// <typeparam name="TSource">The type of the items in the source paginated response.</typeparam>
    /// <typeparam name="TDest">The type of the items in the resulting paginated response.</typeparam>
    /// <param name="source">The source paginated response containing items of type <typeparamref name="TSource"/> to be mapped.</param>
    /// <param name="mapper">A function that defines how to map each item of type <typeparamref name="TSource"/> to type <typeparamref
    /// name="TDest"/>.</param>
    /// <returns>A new <see cref="PaginatedResponse{TDest}"/> containing the mapped items and the same pagination metadata as the
    /// source.</returns>
    public static PaginatedResponse<TDest> MapItems<TSource, TDest>(
        this PaginatedResponse<TSource> source,
        Func<TSource, TDest> mapper)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(mapper);
        return new PaginatedResponse<TDest>
        {
            Items = [.. source.Items.Select(mapper)],
            TotalCount = source.TotalCount,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            TotalPages = source.TotalPages,
        };
    }
}

/// <summary>
/// Generic paginated response wrapper for API endpoints.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public sealed class PaginatedResponse<T>
{
    /// <summary>
    /// The items for the current page.
    /// </summary>
    public T[] Items { get; set; } = [];

    /// <summary>
    /// Indicates whether there is a next page of results.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public long PageNumber { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public long PageSize { get; set; }

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public long TotalPages { get; set; }
}
