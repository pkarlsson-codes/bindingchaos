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

    /// <summary>Raises a new inquiry against a project.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="request">The raise inquiry request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created inquiry identifier.</returns>
    Task<string> RaiseProjectInquiryAsync(
        string projectId,
        RaiseProjectInquiryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Lists inquiries for a project.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="query">Pagination and sort specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of inquiries.</returns>
    Task<PaginatedResponse<ProjectInquiryResponse>> GetProjectInquiriesAsync(
        string projectId,
        PaginationQuerySpec query,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a single project inquiry.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The inquiry response.</returns>
    Task<ProjectInquiryResponse> GetProjectInquiryAsync(
        string projectId,
        string inquiryId,
        CancellationToken cancellationToken = default);

    /// <summary>Submits a user group response to an inquiry.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">The response request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RespondToProjectInquiryAsync(
        string projectId,
        string inquiryId,
        RespondToProjectInquiryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Resolves an inquiry, accepting the response.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ResolveProjectInquiryAsync(
        string projectId,
        string inquiryId,
        CancellationToken cancellationToken = default);

    /// <summary>Updates the inquiry body, resetting it to open.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateProjectInquiryAsync(
        string projectId,
        string inquiryId,
        UpdateProjectInquiryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Reopens a lapsed inquiry, optionally with an updated body.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">Optional updated body request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ReopenProjectInquiryAsync(
        string projectId,
        string inquiryId,
        UpdateProjectInquiryRequest? request,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the contestation status of a project.</summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contestation status response.</returns>
    Task<ProjectContestationStatusResponse> GetProjectContestationStatusAsync(
        string projectId,
        CancellationToken cancellationToken = default);
}
