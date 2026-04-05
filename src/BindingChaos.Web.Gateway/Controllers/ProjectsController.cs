using BindingChaos.CorePlatform.Clients;
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
    /// <param name="querySpec">The pagination and sorting specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response containing project list items.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProjectListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getProjectsForUserGroup")]
    public async Task<IActionResult> GetProjectsForUserGroup(
        [FromQuery] string userGroupId,
        [FromQuery] PaginationQuerySpec querySpec,
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
}
