using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of the Societies API client.
/// </summary>
/// <param name="httpClient">The HTTP client to use for API requests.</param>
/// <param name="logger">The logger for this client.</param>
public class SocietiesApiClient(HttpClient httpClient, ILogger<SocietiesApiClient> logger) : BaseApiClient(httpClient, logger), ISocietiesApiClient
{
    /// <inheritdoc/>
    public async Task<PaginatedResponse<SocietyListItemResponse>> GetSocietiesAsync(PaginationQuerySpec<SocietiesQueryFilter> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return await GetAsync<PaginatedResponse<SocietyListItemResponse>>($"api/societies{queryString}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<SocietyResponse> GetSocietyAsync(string societyId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        return GetAsync<SocietyResponse>($"api/societies/{societyId}", cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponse<SocietyMemberResponse>> GetSocietyMembersAsync(string societyId, PaginationQuerySpec<EmptyFilter> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return await GetAsync<PaginatedResponse<SocietyMemberResponse>>($"api/societies/{societyId}/members{queryString}", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<string> CreateSocietyAsync(CreateSocietyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<CreateSocietyRequest, string>("api/societies", request, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<string> JoinSocietyAsync(string societyId, JoinSocietyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<JoinSocietyRequest, string>($"api/societies/{societyId}/memberships", request, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<string[]> GetMySocietyIdsAsync(CancellationToken cancellationToken = default)
        => GetAsync<string[]>("api/societies/memberships/me", cancellationToken);

    /// <inheritdoc/>
    public Task LeaveSocietyAsync(string societyId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        return DeleteAsync($"api/societies/{societyId}/memberships/me", cancellationToken);
    }
}
