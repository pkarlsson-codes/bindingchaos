using BindingChaos.CommunityDiscourse.Application.Commands;
using BindingChaos.CommunityDiscourse.Application.DTOs;
using BindingChaos.CommunityDiscourse.Application.Queries;
using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CommunityDiscourse.Domain.Contributions;
using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Mappings;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Wolverine;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing community discourse around entities.
/// </summary>
/// <param name="messageBus">The mediator for dispatching commands and queries.</param>
[ApiController]
[Route("api/discourse")]
public sealed class DiscourseController(IMessageBus messageBus) : BaseApiController
{
    /// <summary>
    /// Gets root contributions for a specific entity with cursor-based pagination.
    /// </summary>
    /// <param name="threadId">The ID of the thread to get contributions for.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of contributions to return (default: 5).</param>
    /// <param name="direction">Direction of pagination (forward or backward).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The root contributions for the entity with cursor pagination.</returns>
    [HttpGet("threads/{threadId}/contributions")]
    [ProducesResponseType(typeof(ApiResponse<GetContributionsResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [EndpointName("getContributions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetContributions(
        [FromRoute] string threadId,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 5,
        [FromQuery] CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(threadId);

        var query = new GetRootContributions(threadId, cursor, limit, direction);
        var result = await messageBus.InvokeAsync<RootContributionsView?>(query, cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return NotFound($"No thread found with id {threadId}.");
        }

        return Ok(DiscourseMapper.ToGetContributionsResponse(result, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Gets replies for a specific contribution with cursor-based pagination.
    /// </summary>
    /// <param name="contributionId">The ID of the contribution to get replies for.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of replies to return (default: 5).</param>
    /// <param name="direction">Direction of pagination (forward or backward).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The replies for the contribution with cursor pagination.</returns>
    [HttpGet("contributions/{contributionId}/replies")]
    [ProducesResponseType(typeof(ApiResponse<GetContributionRepliesResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [EndpointName("getContributionReplies")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReplies(
        [FromRoute] string contributionId,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 5,
        [FromQuery] CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contributionId);

        var query = new GetContributionReplies(contributionId, cursor, limit, direction);
        var result = await messageBus.InvokeAsync<ContributionRepliesView?>(query, cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return NotFound($"Contribution with ID {contributionId} not found.");
        }

        return Ok(DiscourseMapper.ToGetContributionRepliesResponse(result, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Posts a new contribution to the discourse thread for an entity.
    /// </summary>
    /// <param name="threadId">The ID of the entity.</param>
    /// <param name="request">The contribution posting request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created contribution.</returns>
    [HttpPost("threads/{threadId}/contributions")]
    [ProducesResponseType(typeof(ApiResponse<PostContributionResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [EndpointName("postContribution")]
    public async Task<IActionResult> PostContribution(
        [FromRoute] string threadId,
        [FromBody] PostContributionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(threadId);
        ArgumentNullException.ThrowIfNull(request);

        var authorId = HttpContext.GetParticipantIdOrAnonymous();

        ContributionId? parentContributionId = null;
        if (!string.IsNullOrEmpty(request.ParentContributionId))
        {
            parentContributionId = ContributionId.Create(request.ParentContributionId);
            if (parentContributionId == null)
            {
                return BadRequest($"Invalid parent contribution ID format: {request.ParentContributionId}");
            }
        }

        var command = new PostContribution(
            threadId,
            authorId,
            request.Content,
            parentContributionId);

        var contributionId = await messageBus.InvokeAsync<ContributionId>(command, cancellationToken).ConfigureAwait(false);

        var response = new PostContributionResponse(contributionId.Value);
        return CreatedAtAction(
            nameof(GetContributions),
            new { threadId },
            response);
    }

    /// <summary>
    /// Gets root contributions for a specific entity with cursor-based pagination.
    /// </summary>
    /// <param name="entityType">The type of the entity for which the discourse thread is being retrieved. Cannot be null or whitespace.</param>
    /// <param name="entityId">The unique identifier of the entity for which the discourse thread is being retrieved. Cannot be null or
    /// whitespace.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of contributions to return (default: 5).</param>
    /// <param name="direction">Direction of pagination (forward or backward).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> with the retrieved <see
    /// cref="GetContributionsResponse"/> if found, or a 404 response if no thread is associated with the specified entity.</returns>
    [HttpGet("threads/by-entity/{entityType}/{entityId}/contributions")]
    [ProducesResponseType(typeof(ApiResponse<GetContributionsResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [EndpointName("getContributionsByEntity")]
    [AllowAnonymous]
    public async Task<IActionResult> GetContributionsByEntity(
        [FromRoute] string entityType,
        [FromRoute] string entityId,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 5,
        [FromQuery] CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityType);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityId);

        var threadQuery = new GetThreadByEntity(entityType, entityId);
        var thread = await messageBus.InvokeAsync<DiscourseThreadView?>(threadQuery, cancellationToken).ConfigureAwait(false);
        if (thread == null)
        {
            return NotFound($"No discourse thread found for entity type '{entityType}' with ID '{entityId}'.");
        }

        var contributionsQuery = new GetRootContributions(thread.Id, cursor, limit, direction);
        var result = await messageBus.InvokeAsync<RootContributionsView?>(contributionsQuery, cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            return NotFound($"No thread found with id {thread.Id}.");
        }

        return Ok(DiscourseMapper.ToGetContributionsResponse(result, HttpContext.TraceIdentifier, thread.Id));
    }
}
