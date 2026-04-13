using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of <see cref="IUserGroupsApiClient"/> that communicates with the user groups API endpoints.
/// </summary>
/// <param name="httpClient">The HTTP client used to make API requests.</param>
/// <param name="logger">The logger used to log API client operations.</param>
public sealed class UserGroupsApiClient(
    HttpClient httpClient,
    ILogger<UserGroupsApiClient> logger) : BaseApiClient(httpClient, logger), IUserGroupsApiClient
{
    /// <inheritdoc />
    public async Task<string> FormUserGroupAsync(
        FormUserGroupRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await PostAsync<FormUserGroupRequest, string>(
            "api/usergroups",
            request,
            cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<UserGroupListItemResponse>> GetUserGroupsForCommonsAsync(
        string commonsId,
        PaginationQuerySpec querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commonsId);
        ArgumentNullException.ThrowIfNull(querySpec);

        var paginationQuery = querySpec.ToQueryString(false);
        var url = $"api/usergroups?commonsId={Uri.EscapeDataString(commonsId)}&{paginationQuery}";

        return await GetAsync<PaginatedResponse<UserGroupListItemResponse>>(url, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserGroupListItemResponse>> GetMyUserGroupsAsync(
        CancellationToken cancellationToken)
    {
        return await GetAsync<IReadOnlyList<UserGroupListItemResponse>>("api/usergroups/mine", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<UserGroupListItemResponse[]> GetUserGroupsForParticipantAsync(
        string participantId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(participantId);

        return await GetAsync<UserGroupListItemResponse[]>(
            $"api/usergroups/for-participant?participantId={Uri.EscapeDataString(participantId)}",
            cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<UserGroupDetailResponse?> GetUserGroupDetailAsync(
        string userGroupId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userGroupId);

        var response = await HttpClient
            .GetAsync($"api/usergroups/{Uri.EscapeDataString(userGroupId)}", cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse<UserGroupDetailResponse>>(JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        return apiResponse?.Data;
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<UserGroupMemberResponse>> GetUserGroupMembersAsync(
        string userGroupId,
        PaginationQuerySpec querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userGroupId);
        ArgumentNullException.ThrowIfNull(querySpec);

        var paginationQuery = querySpec.ToQueryString(false);
        var url = $"api/usergroups/{Uri.EscapeDataString(userGroupId)}/members?{paginationQuery}";

        return await GetAsync<PaginatedResponse<UserGroupMemberResponse>>(url, cancellationToken)
            .ConfigureAwait(false);
    }
}
