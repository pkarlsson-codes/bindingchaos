using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Mappings;
using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Queries;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing projects.
/// </summary>
/// <param name="messageBus">The message bus instance used for dispatching commands and queries.</param>
/// <param name="querySession">The Marten query session for direct read-model access.</param>
[ApiController]
[Route("api/projects")]
public sealed class ProjectsController(IMessageBus messageBus, IQuerySession querySession) : BaseApiController
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
    /// <param name="queryRequest">Pagination, sorting, and filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of projects.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProjectListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [EndpointName("getProjectsForUserGroup")]
    public async Task<IActionResult> GetProjectsForUserGroup(
        [FromQuery] string userGroupId,
        [FromQuery] PaginationQuerySpec<ProjectsQueryFilter> queryRequest,
        CancellationToken cancellationToken)
    {
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

        return Created($"/api/projects/{projectId}/amendments/{amendmentId.Value}", amendmentId.Value);
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

    /// <summary>
    /// Raises a new inquiry against a project on behalf of an affected society member.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="request">The raise inquiry request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created inquiry identifier.</returns>
    [HttpPost("{projectId}/inquiries")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("raiseProjectInquiry")]
    public async Task<IActionResult> RaiseProjectInquiry(
        [FromRoute] string projectId,
        [FromBody] RaiseProjectInquiryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new RaiseProjectInquiry(ProjectId.Create(projectId), participantId, request.Body);
        var inquiryId = await messageBus.InvokeAsync<ProjectInquiryId>(command, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetProjectInquiry), new { projectId, inquiryId = inquiryId.Value }, inquiryId.Value);
    }

    /// <summary>
    /// Lists inquiries for a specific project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="querySpec">Pagination and sorting parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of inquiries.</returns>
    [HttpGet("{projectId}/inquiries")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProjectInquiryResponse>>), StatusCodes.Status200OK)]
    [EndpointName("getProjectInquiries")]
    public async Task<IActionResult> GetProjectInquiries(
        [FromRoute] string projectId,
        [FromQuery] PaginationQuerySpec querySpec,
        CancellationToken cancellationToken)
    {
        var currentParticipantId = HttpContext.GetParticipantIdOrAnonymous();
        var query = new GetProjectInquiries(ProjectId.Create(projectId), querySpec);
        var result = await messageBus.InvokeAsync<PaginatedResponse<ProjectInquiryView>>(query, cancellationToken).ConfigureAwait(false);
        var isGroupMember = await IsCurrentUserInProjectUserGroupAsync(projectId, currentParticipantId, cancellationToken).ConfigureAwait(false);
        return Ok(result.MapItems(v => ToInquiryResponse(v, currentParticipantId, isGroupMember)));
    }

    /// <summary>
    /// Gets a specific project inquiry.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The inquiry details when found.</returns>
    [HttpGet("{projectId}/inquiries/{inquiryId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProjectInquiryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("getProjectInquiry")]
    public async Task<IActionResult> GetProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        CancellationToken cancellationToken)
    {
        var currentParticipantId = HttpContext.GetParticipantIdOrAnonymous();
        var query = new GetProjectInquiry(ProjectInquiryId.Create(inquiryId));
        var view = await messageBus.InvokeAsync<ProjectInquiryView?>(query, cancellationToken).ConfigureAwait(false);
        if (view is null)
        {
            return NotFound();
        }

        var isGroupMember = await IsCurrentUserInProjectUserGroupAsync(projectId, currentParticipantId, cancellationToken).ConfigureAwait(false);
        return Ok(ToInquiryResponse(view, currentParticipantId, isGroupMember));
    }

    /// <summary>
    /// Submits a response to a project inquiry on behalf of a user group member.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">The response request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{projectId}/inquiries/{inquiryId}/responses")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("respondToProjectInquiry")]
    public async Task<IActionResult> RespondToProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        [FromBody] RespondToProjectInquiryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new RespondToProjectInquiry(
            ProjectInquiryId.Create(inquiryId),
            ProjectId.Create(projectId),
            participantId,
            request.Response);
        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Resolves a project inquiry, accepting the user group's response.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{projectId}/inquiries/{inquiryId}/resolutions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("resolveProjectInquiry")]
    public async Task<IActionResult> ResolveProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new ResolveProjectInquiry(ProjectInquiryId.Create(inquiryId), participantId);
        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Updates the body of a project inquiry, resetting it to open status.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPatch("{projectId}/inquiries/{inquiryId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("updateProjectInquiry")]
    public async Task<IActionResult> UpdateProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        [FromBody] UpdateProjectInquiryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new UpdateProjectInquiry(ProjectInquiryId.Create(inquiryId), participantId, request.NewBody);
        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Reopens a lapsed inquiry, optionally with an updated body.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="inquiryId">The inquiry identifier.</param>
    /// <param name="request">Optional updated body request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{projectId}/inquiries/{inquiryId}/reopenings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointName("reopenProjectInquiry")]
    public async Task<IActionResult> ReopenProjectInquiry(
        [FromRoute] string projectId,
        [FromRoute] string inquiryId,
        [FromBody] UpdateProjectInquiryRequest? request,
        CancellationToken cancellationToken)
    {
        var participantId = HttpContext.GetParticipantIdOrAnonymous();
        if (participantId == ParticipantId.Anonymous)
        {
            return Unauthorized();
        }

        var command = new ReopenProjectInquiry(ProjectInquiryId.Create(inquiryId), participantId, request?.NewBody);
        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Gets the contestation status of a project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contestation status.</returns>
    [HttpGet("{projectId}/contestation-status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProjectContestationStatusResponse>), StatusCodes.Status200OK)]
    [EndpointName("getProjectContestationStatus")]
    public async Task<IActionResult> GetProjectContestationStatus(
        [FromRoute] string projectId,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectContestationStatus(ProjectId.Create(projectId));
        var view = await messageBus.InvokeAsync<ProjectContestationStatusView>(query, cancellationToken).ConfigureAwait(false);
        return Ok(new ProjectContestationStatusResponse(view.OpenInquiryCount, view.IsContested));
    }

    private static ProjectInquiryResponse ToInquiryResponse(ProjectInquiryView v, ParticipantId currentParticipantId, bool isGroupMember) =>
        new(
            v.Id,
            v.ProjectId,
            v.RaisedByParticipantId,
            v.RaisedBySocietyId,
            v.Body,
            v.Status,
            v.Response,
            v.DiscourseThreadId,
            v.RaisedAt,
            v.LastUpdatedAt,
            IsRaisedByCurrentUser: !currentParticipantId.IsAnonymous && currentParticipantId.Value == v.RaisedByParticipantId,
            IsCurrentUserInProjectUserGroup: isGroupMember);

    private async Task<bool> IsCurrentUserInProjectUserGroupAsync(string projectId, ParticipantId currentParticipantId, CancellationToken cancellationToken)
    {
        if (currentParticipantId.IsAnonymous)
        {
            return false;
        }

        var project = await querySession.LoadAsync<ProjectView>(projectId, cancellationToken).ConfigureAwait(false);
        if (project is null)
        {
            return false;
        }

        var membershipKey = $"{project.UserGroupId}:{currentParticipantId.Value}";
        var membership = await querySession.LoadAsync<UserGroupMembersView>(membershipKey, cancellationToken).ConfigureAwait(false);
        return membership is not null;
    }
}
