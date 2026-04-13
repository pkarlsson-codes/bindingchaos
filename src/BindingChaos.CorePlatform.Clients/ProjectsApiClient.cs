using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of <see cref="IProjectsApiClient"/>.
/// </summary>
/// <param name="httpClient">The HTTP client used for API requests.</param>
/// <param name="logger">The logger used for API client operations.</param>
public sealed class ProjectsApiClient(
    HttpClient httpClient,
    ILogger<ProjectsApiClient> logger)
    : BaseApiClient(httpClient, logger), IProjectsApiClient
{
    /// <inheritdoc />
    public Task<string> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return PostAsync<CreateProjectRequest, string>(
            "api/projects",
            request,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<ProjectResponse> GetProjectAsync(string projectId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        return GetAsync<ProjectResponse>(
            $"api/projects/{projectId}",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PaginatedResponse<ProjectListItemResponse>> GetProjectsForUserGroupAsync(
        string userGroupId,
        PaginationQuerySpec<ProjectsQueryFilter> query,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userGroupId);
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);
        var separator = string.IsNullOrWhiteSpace(queryString) ? "?" : "&";

        return GetAsync<PaginatedResponse<ProjectListItemResponse>>(
            $"api/projects{queryString}{separator}userGroupId={Uri.EscapeDataString(userGroupId)}",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<string> ProposeProjectAmendmentAsync(string projectId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        return PostAsync<string>(
            $"api/projects/{projectId}/amendments",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task ContestProjectAmendmentAsync(string projectId, string amendmentId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(amendmentId);

        return PostAsync(
            $"api/projects/{projectId}/amendments/{amendmentId}/contests",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<string> RaiseProjectInquiryAsync(string projectId, RaiseProjectInquiryRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentNullException.ThrowIfNull(request);

        return PostAsync<RaiseProjectInquiryRequest, string>(
            $"api/projects/{projectId}/inquiries",
            request,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<PaginatedResponse<ProjectInquiryResponse>> GetProjectInquiriesAsync(string projectId, PaginationQuerySpec query, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentNullException.ThrowIfNull(query);

        var queryString = query.ToQueryString(true);

        return GetAsync<PaginatedResponse<ProjectInquiryResponse>>(
            $"api/projects/{projectId}/inquiries{queryString}",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<ProjectInquiryResponse> GetProjectInquiryAsync(string projectId, string inquiryId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(inquiryId);

        return GetAsync<ProjectInquiryResponse>(
            $"api/projects/{projectId}/inquiries/{inquiryId}",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task RespondToProjectInquiryAsync(string projectId, string inquiryId, RespondToProjectInquiryRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(inquiryId);
        ArgumentNullException.ThrowIfNull(request);

        return PostAsync(
            $"api/projects/{projectId}/inquiries/{inquiryId}/responses",
            request,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task ResolveProjectInquiryAsync(string projectId, string inquiryId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(inquiryId);

        return PostAsync(
            $"api/projects/{projectId}/inquiries/{inquiryId}/resolutions",
            cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateProjectInquiryAsync(string projectId, string inquiryId, UpdateProjectInquiryRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(inquiryId);
        ArgumentNullException.ThrowIfNull(request);

        return PatchAsync(
            $"api/projects/{projectId}/inquiries/{inquiryId}",
            request,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task ReopenProjectInquiryAsync(string projectId, string inquiryId, UpdateProjectInquiryRequest? request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(inquiryId);

        var endpoint = $"api/projects/{projectId}/inquiries/{inquiryId}/reopenings";
        return request is null
            ? PostAsync(endpoint, cancellationToken)
            : PostAsync(endpoint, request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ProjectContestationStatusResponse> GetProjectContestationStatusAsync(string projectId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        return GetAsync<ProjectContestationStatusResponse>(
            $"api/projects/{projectId}/contestation-status",
            cancellationToken);
    }
}
