using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Mappings;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Queries;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing projects.
/// </summary>
/// <param name="messageBus">The message bus instance used for dispatching commands and queries.</param>
[ApiController]
[Route("api/projects")]
public sealed class ProjectsController(IMessageBus messageBus) : BaseApiController
{
    /// <summary>
    /// Creates a new project in a user group.
    /// </summary>
    /// <param name="request">The project creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created project identifier.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("createProject")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new CreateProject(
            UserGroupId.Create(request.UserGroupId),
            participantId,
            request.Title,
            request.Description);

        var projectId = await messageBus.InvokeAsync<ProjectId>(command, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetProject), new { projectId = projectId.Value }, projectId.Value);
    }

    /// <summary>
    /// Gets a specific project by its identifier.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The project details when found.</returns>
    [HttpGet("{projectId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("getProject")]
    public async Task<IActionResult> GetProject([FromRoute] string projectId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        var query = new GetProject(ProjectId.Create(projectId));
        var project = await messageBus.InvokeAsync<ProjectView?>(query, cancellationToken).ConfigureAwait(false);
        if (project is null)
        {
            return NotFound($"Project with ID {projectId} not found.");
        }

        return Ok(ProjectMapper.ToProjectResponse(project));
    }

    /// <summary>
    /// Lists projects for a specific user group.
    /// </summary>
    /// <param name="userGroupId">The owning user group identifier.</param>
    /// <param name="queryRequest">Pagination and sorting parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of projects.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProjectListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointName("getProjectsForUserGroup")]
    public async Task<IActionResult> GetProjectsForUserGroup(
        [FromQuery] string userGroupId,
        [FromQuery] PaginationQuerySpec queryRequest,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userGroupId))
        {
            return BadRequest("userGroupId is required.");
        }

        var query = new GetProjectsForUserGroup(UserGroupId.Create(userGroupId), queryRequest);
        var projects = await messageBus.InvokeAsync<PaginatedResponse<ProjectsListItemView>>(query, cancellationToken).ConfigureAwait(false);

        return Ok(projects.MapItems(ProjectMapper.ToProjectListItemResponse));
    }

    /// <summary>
    /// Proposes a new amendment for a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created amendment identifier.</returns>
    [HttpPost("{projectId}/amendments")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("proposeProjectAmendment")]
    public async Task<IActionResult> ProposeProjectAmendment([FromRoute] string projectId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new ProposeProjectAmendment(ProjectId.Create(projectId), participantId);
        var amendmentId = await messageBus.InvokeAsync<AmendmentId>(command, cancellationToken).ConfigureAwait(false);

        return Created($"api/projects/{projectId}/amendments/{amendmentId.Value}", amendmentId.Value);
    }

    /// <summary>
    /// Contests an active amendment for a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="amendmentId">The amendment identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content when contention starts successfully.</returns>
    [HttpPost("{projectId}/amendments/{amendmentId}/contests")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("contestProjectAmendment")]
    public async Task<IActionResult> ContestProjectAmendment(
        [FromRoute] string projectId,
        [FromRoute] string amendmentId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(amendmentId);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new ContestAmendment(
            ProjectId.Create(projectId),
            AmendmentId.Create(amendmentId),
            participantId);

        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}
