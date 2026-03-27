using System.Globalization;
using BindingChaos.CorePlatform.Contracts.Responses.Tags;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Provides a client for interacting with the tags API, enabling operations such as creating, retrieving, updating, and
/// deleting tags.
/// </summary>
/// <param name="httpClient">The HTTP client used to send requests to the tags API. Must be configured with the appropriate base address and
/// authentication settings.</param>
/// <param name="logger">The logger used to record information and errors related to tag API operations.</param>
public class TagsApiClient(
    HttpClient httpClient,
    ILogger<TagsApiClient> logger)
    : BaseApiClient(httpClient, logger), ITagsApiClient
{
    /// <inheritdoc/>
    public Task<TagResponse[]> GetTags(
        int limit,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryString.Create(
            "limit",
            limit.ToString(CultureInfo.InvariantCulture));

        if (search != null)
        {
            queryString = queryString.Add("search", search);
        }

        return GetAsync<TagResponse[]>(
            $"api/tags{queryString}",
            cancellationToken);
    }
}
