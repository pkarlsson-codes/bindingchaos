using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of <see cref="IConcernsApiClient"/> that communicates with the concerns API endpoints.
/// </summary>
/// <param name="httpClient">The HTTP client used to make API requests.</param>
/// <param name="logger">The logger used to log API client operations.</param>
public sealed class ConcernsApiClient(
    HttpClient httpClient,
    ILogger<ConcernsApiClient> logger) : BaseApiClient(httpClient, logger), IConcernsApiClient
{
    /// <inheritdoc />
    public async Task<string> RaiseConcernAsync(RaiseConcernRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<RaiseConcernRequest, string>(
            "api/concerns",
            request,
            cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<ConcernListItemResponse>> GetConcernsAsync(
        PaginationQuerySpec querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var queryString = querySpec?.ToQueryString(true) ?? string.Empty;
        return await GetAsync<PaginatedResponse<ConcernListItemResponse>>(
            $"api/concerns{queryString}",
            cancellationToken)
            .ConfigureAwait(false);
    }
}