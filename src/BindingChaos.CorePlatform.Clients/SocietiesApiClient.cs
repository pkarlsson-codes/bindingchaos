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
public class SocietiesApiClient(
    HttpClient httpClient,
    ILogger<SocietiesApiClient> logger)
    : BaseApiClient(httpClient, logger), ISocietiesApiClient
{
    /// <inheritdoc/>
    public Task<PaginatedResponse<SocietyListItemResponse>> GetSocietiesAsync(
        PaginationQuerySpec<SocietiesQueryFilter> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return GetAsync<PaginatedResponse<SocietyListItemResponse>>(
            $"api/societies{queryString}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<SocietyResponse> GetSocietyAsync(
        string societyId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        return GetAsync<SocietyResponse>(
            $"api/societies/{societyId}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<PaginatedResponse<SocietyMemberResponse>> GetSocietyMembersAsync(
        string societyId,
        PaginationQuerySpec<EmptyFilter> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        return GetAsync<PaginatedResponse<SocietyMemberResponse>>(
            $"api/societies/{societyId}/members{queryString}",
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<string> CreateSocietyAsync(
        CreateSocietyRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<CreateSocietyRequest, string>(
            "api/societies",
            request,
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<string> JoinSocietyAsync(
        string societyId,
        JoinSocietyRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<JoinSocietyRequest, string>(
            $"api/societies/{societyId}/memberships",
            request,
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<string[]> GetMySocietyIdsAsync(
        CancellationToken cancellationToken)
        => GetAsync<string[]>(
            "api/societies/memberships/me",
            cancellationToken);

    /// <inheritdoc/>
    public Task LeaveSocietyAsync(
        string societyId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        return DeleteAsync(
            $"api/societies/{societyId}/memberships/me",
            cancellationToken);
    }
}
