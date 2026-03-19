namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Page parameters for offset-based pagination (bound from querystring via page.* keys).
/// </summary>
public sealed class PageSpec
{
    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int Number { get; init; } = 1;

    /// <summary>
    /// The requested page size.
    /// </summary>
    public int Size { get; init; } = PaginationQuerySpec<object>.DefaultPageSize;
}
