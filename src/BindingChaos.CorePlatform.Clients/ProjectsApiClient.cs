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
        PaginationQuerySpec query,
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
}
