using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of the Ideas API client.
/// </summary>
/// <param name="httpClient">The HTTP client to use for API requests.</param>
/// <param name="logger">The logger for this client.</param>
public class IdeasApiClient(HttpClient httpClient, ILogger<IdeasApiClient> logger) : BaseApiClient(httpClient, logger), IIdeasApiClient
{
    /// <inheritdoc/>
    public Task<string> AuthorIdeaAsync(AuthorIdeaRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<AuthorIdeaRequest, string>("api/ideas", request, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IdeaResponse> GetIdeaAsync(string ideaId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ideaId);
        return GetAsync<IdeaResponse>($"api/ideas/{ideaId}", cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponse<IdeaListItemResponse>> GetIdeasAsync(PaginationQuerySpec<IdeasQueryFilter> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return await GetAsync<PaginatedResponse<IdeaListItemResponse>>($"api/ideas{queryString}", cancellationToken).ConfigureAwait(false);
    }
}