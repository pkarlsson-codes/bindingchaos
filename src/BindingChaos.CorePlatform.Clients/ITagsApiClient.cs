using BindingChaos.CorePlatform.Contracts.Responses.Tags;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Defines methods for retrieving and filtering global tags.
/// </summary>
public interface ITagsApiClient
{
    /// <summary>
    /// Retrieves a collection of global tags, optionally filtered by a search term.
    /// </summary>
    /// <param name="limit">The maximum number of tags to return. Must be a positive integer.</param>
    /// <param name="search">An optional search term used to filter the tags. If specified, only tags matching this term are included in the
    /// results.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Some tags.</returns>
    public Task<TagResponse[]> GetTags(
        int limit,
        string? search = null,
        CancellationToken cancellationToken = default);
}