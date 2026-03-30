using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of the Emerging Patterns API client.
/// </summary>
/// <param name="httpClient">The HTTP client to use for API requests.</param>
/// <param name="logger">The logger for this client.</param>
public class EmergingPatternsApiClient(
    HttpClient httpClient,
    ILogger<EmergingPatternsApiClient> logger)
    : BaseApiClient(httpClient, logger), IEmergingPatternsApiClient
{
    /// <inheritdoc/>
    public Task<EmergingPatternsResponse> GetEmergingPatternsAsync(
        CancellationToken cancellationToken)
    {
        return GetAsync<EmergingPatternsResponse>(
            "api/emerging-patterns",
            cancellationToken);
    }
}
