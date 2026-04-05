using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of <see cref="IProfilesApiClient"/> that communicates with the profiles API endpoints.
/// </summary>
/// <param name="httpClient">The HTTP client used to make API requests.</param>
/// <param name="logger">The logger used to log API client operations.</param>
public sealed class ProfilesApiClient(
    HttpClient httpClient,
    ILogger<ProfilesApiClient> logger) : BaseApiClient(httpClient, logger), IProfilesApiClient
{
    /// <inheritdoc />
    public async Task<ParticipantProfileResponse?> GetProfileAsync(
        string pseudonym,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pseudonym);

        var response = await HttpClient
            .GetAsync($"api/profiles/{Uri.EscapeDataString(pseudonym)}", cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse<ParticipantProfileResponse>>(JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return apiResponse?.Data;
    }
}
