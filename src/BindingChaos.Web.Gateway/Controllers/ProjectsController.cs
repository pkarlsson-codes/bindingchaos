using BindingChaos.CorePlatform.Clients;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller responsible for project lifecycle operations and forwarding requests to the projects API client.
/// </summary>
/// <param name="projectsApiClient">The API client used to forward project-related requests to the core platform API.</param>
[ApiController]
[Route("api/v1/projects")]
public sealed class ProjectsController(IProjectsApiClient projectsApiClient) : BaseApiController
{
    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The project creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created project identifier.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("createProject")]
    public async Task<IActionResult> CreateProject(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var projectId = await projectsApiClient
            .CreateProjectAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return Created(string.Empty, projectId);
    }

    /// <summary>
    /// Retrieves a single project by identifier.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The project details.</returns>
    [HttpGet("{projectId}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("getProject")]
    public async Task<IActionResult> GetProject(
        [FromRoute] string projectId,
        CancellationToken cancellationToken)
    {
        var project = await projectsApiClient
            .GetProjectAsync(projectId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(project);
    }

    /// <summary>
    /// Retrieves a paginated list of projects for the specified user group.
    /// </summary>
    /// <param name="userGroupId">The user group identifier to filter by.</param>
    /// <param name="querySpec">The pagination, sorting, and filter specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response containing project list items.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProjectListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getProjectsForUserGroup")]
    public async Task<IActionResult> GetProjectsForUserGroup(
        [FromQuery] string userGroupId,
        [FromQuery] PaginationQuerySpec<ProjectsQueryFilter> querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var projects = await projectsApiClient
            .GetProjectsForUserGroupAsync(userGroupId, querySpec, cancellationToken)
            .ConfigureAwait(false);

        return Ok(projects);
    }

    /// <summary>
    /// Proposes a new amendment for a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created amendment identifier.</returns>
    [HttpPost("{projectId}/amendments")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [EndpointName("proposeProjectAmendment")]
    public async Task<IActionResult> ProposeProjectAmendment(
        [FromRoute] string projectId,
        CancellationToken cancellationToken)
    {
        var amendmentId = await projectsApiClient
            .ProposeProjectAmendmentAsync(projectId, cancellationToken)
            .ConfigureAwait(false);

        return Created(string.Empty, amendmentId);
    }

    /// <summary>
    /// Contests an active amendment on a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="amendmentId">The amendment identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpPost("{projectId}/amendments/{amendmentId}/contests")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [EndpointName("contestProjectAmendment")]
    public async Task<IActionResult> ContestProjectAmendment(
        [FromRoute] string projectId,
        [FromRoute] string amendmentId,
        CancellationToken cancellationToken)
    {
        await projectsApiClient
            .ContestProjectAmendmentAsync(projectId, amendmentId, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Raises a new inquiry against a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="request">The raise inquiry request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created inquiry identifier.</returns>
    [HttpPost("{projectId}/inquiries")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [EndpointName("raiseProjectInquiry")]
    public async Task<IActionResult> RaiseProjectInquiry(
        [FromRoute] string projectId,
        [FromBody] RaiseProjectInquiryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var inquiryId = await projectsApiClient
            .RaiseProjectInquiryAsync(projectId, request, cancellationToken)
            .ConfigureAwait(false);

        return Created(string.Empty, inquiryId);
    }

    /// <summary>
    /// Lists inquiries for a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="querySpec">Pagination and sort specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of inquiries.</returns>
    [HttpGet("{projectId}/inquiries")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProjectInquiryResponse>>), StatusCodes.Status200OK)]
    [EndpointName("getProjectInquiries")]
    public async Task<IActionResult> GetProjectInquiries(
        [FromRoute] string projectId,
        [FromQuery] PaginationQuerySpec querySpec,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(querySpec);

        var result = await projectsApiClient
            .GetProjectInquiriesAsync(projectId, querySpec, cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>
    /// Gets a single project inquiry.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The inquiry details.</returns>
    [HttpGet("{projectId}/inquiries/{inquiryId}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectInquiryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("getProjectInquiry")]
    public async Task<IActionResult> GetProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        CancellationToken cancellationToken)
    {
        var inquiry = await projectsApiClient
            .GetProjectInquiryAsync(projectId, inquiryId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(inquiry);
    }

    /// <summary>
    /// Submits a user group response to an inquiry.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">The response request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{projectId}/inquiries/{inquiryId}/responses")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [EndpointName("respondToProjectInquiry")]
    public async Task<IActionResult> RespondToProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        [FromBody] RespondToProjectInquiryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await projectsApiClient
            .RespondToProjectInquiryAsync(projectId, inquiryId, request, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Resolves an inquiry, accepting the user group's response.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{projectId}/inquiries/{inquiryId}/resolutions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [EndpointName("resolveProjectInquiry")]
    public async Task<IActionResult> ResolveProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        CancellationToken cancellationToken)
    {
        await projectsApiClient
            .ResolveProjectInquiryAsync(projectId, inquiryId, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Updates the body of an inquiry, resetting it to open status.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPatch("{projectId}/inquiries/{inquiryId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [EndpointName("updateProjectInquiry")]
    public async Task<IActionResult> UpdateProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        [FromBody] UpdateProjectInquiryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await projectsApiClient
            .UpdateProjectInquiryAsync(projectId, inquiryId, request, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Reopens a lapsed inquiry, optionally with an updated body.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">Optional updated body.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{projectId}/inquiries/{inquiryId}/reopenings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [EndpointName("reopenProjectInquiry")]
    public async Task<IActionResult> ReopenProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        [FromBody] UpdateProjectInquiryRequest? request,
        CancellationToken cancellationToken)
    {
        await projectsApiClient
            .ReopenProjectInquiryAsync(projectId, inquiryId, request, cancellationToken)
            .ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Gets the contestation status of a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contestation status.</returns>
    [HttpGet("{projectId}/contestation-status")]
    [ProducesResponseType(typeof(ApiResponse<ProjectContestationStatusResponse>), StatusCodes.Status200OK)]
    [EndpointName("getProjectContestationStatus")]
    public async Task<IActionResult> GetProjectContestationStatus(
        [FromRoute] string projectId,
        CancellationToken cancellationToken)
    {
        var status = await projectsApiClient
            .GetProjectContestationStatusAsync(projectId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(status);
    }
}
