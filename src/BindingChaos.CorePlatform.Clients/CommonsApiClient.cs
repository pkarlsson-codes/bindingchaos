using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of <see cref="ICommonsApiClient"/> that communicates with the commons API endpoints.
/// </summary>
/// <param name="httpClient">The HTTP client used to make API requests.</param>
/// <param name="logger">The logger used to log API client operations.</param>
public sealed class CommonsApiClient(
    HttpClient httpClient,
    ILogger<CommonsApiClient> logger) : BaseApiClient(httpClient, logger), ICommonsApiClient
{
    /// <inheritdoc />
    public async Task<PaginatedResponse<CommonsListItemResponse>> GetCommonsAsync(
        PaginationQuerySpec querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var queryString = querySpec.ToQueryString(true);
        return await GetAsync<PaginatedResponse<CommonsListItemResponse>>(
            $"api/commons{queryString}",
            cancellationToken)
            .ConfigureAwait(false);
    }
}
