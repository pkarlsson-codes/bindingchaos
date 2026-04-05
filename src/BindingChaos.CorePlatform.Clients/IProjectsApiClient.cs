using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client interface for interacting with the Projects API.
/// </summary>
public interface IProjectsApiClient
{
    /// <summary>
    /// Gets a paginated list of projects for a user group.
    /// </summary>
    /// <param name="userGroupId">The user group identifier.</param>
    /// <param name="query">The pagination and sorting query spec.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A page of projects.</returns>
    Task<PaginatedResponse<ProjectListItemResponse>> GetProjectsForUserGroupAsync(
        string userGroupId,
        PaginationQuerySpec query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific project by identifier.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The project response.</returns>
    Task<ProjectResponse> GetProjectAsync(
        string projectId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The project creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created project identifier.</returns>
    Task<string> CreateProjectAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Proposes a new amendment for a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created amendment identifier.</returns>
    Task<string> ProposeProjectAmendmentAsync(
        string projectId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Contests an active amendment on a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="amendmentId">The amendment identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ContestProjectAmendmentAsync(
        string projectId,
        string amendmentId,
        CancellationToken cancellationToken = default);
}
