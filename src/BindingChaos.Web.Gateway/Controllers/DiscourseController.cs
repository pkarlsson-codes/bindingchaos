using BindingChaos.CorePlatform.Clients;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Web.Gateway.Mappings;
using BindingChaos.Web.Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using PostContributionRequest = BindingChaos.CorePlatform.Contracts.Requests.PostContributionRequest;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for managing community discourse in the web gateway.
/// Provides cursor-based pagination for optimal infinite scroll performance.
/// </summary>
/// <param name="discourseApiClient">The discourse API client.</param>
[ApiController]
[Route("api/discourse")]
public sealed class DiscourseController(
    IDiscourseApiClient discourseApiClient)
    : BaseApiController
{
    /// <summary>
    /// Gets root posts for a specific thread with cursor-based pagination.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of posts to return (default: 5).</param>
    /// <param name="direction">Direction of pagination (default: forward).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The root posts for the thread with cursor pagination.</returns>
    [HttpGet("threads/{threadId}/posts")]
    [ProducesResponseType(typeof(PostsViewModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointName("getPostsByThreadId")]
    public async Task<IActionResult> GetPostsByThreadId(
        string threadId,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 5,
        [FromQuery] CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        var result = await discourseApiClient
            .GetContributionsByThreadIdAsync(threadId, cursor, limit, direction, cancellationToken);
        return Ok(DiscourseMapper.ToPostsViewModel(result));
    }

    /// <summary>
    /// Gets root posts for a thread identified by entity with cursor-based pagination.
    /// </summary>
    /// <param name="entityType">The type of entity (e.g., "idea", "signal", "action").</param>
    /// <param name="entityId">The ID of the entity.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of posts to return (default: 5).</param>
    /// <param name="direction">Direction of pagination (default: forward).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The root posts for the thread with cursor pagination.</returns>
    [HttpGet("threads/by-entity/{entityType}/{entityId}/posts")]
    [ProducesResponseType(typeof(PostsViewModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointName("getPostsByEntity")]
    public async Task<IActionResult> GetPostsByEntity(
        string entityType,
        string entityId,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 5,
        [FromQuery] CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        var result = await discourseApiClient
            .GetContributionsByEntityAsync(entityType, entityId, cursor, limit, direction, cancellationToken);
        return Ok(DiscourseMapper.ToPostsViewModel(result));
    }

    /// <summary>
    /// Gets replies to a specific post with cursor-based pagination.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="postId">The ID of the post to get replies for.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of replies to return (default: 5).</param>
    /// <param name="direction">Direction of pagination (default: forward).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The replies for the post with cursor pagination.</returns>
    [HttpGet("threads/{threadId}/posts/{postId}/replies")]
    [ProducesResponseType(typeof(PostRepliesViewModel), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointName("getPostReplies")]
    public async Task<IActionResult> GetPostReplies(
        string threadId,
        string postId,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 5,
        [FromQuery] CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default)
    {
        var result = await discourseApiClient
            .GetContributionRepliesAsync(postId, cursor, limit, direction, cancellationToken);

        return Ok(DiscourseMapper.ToPostRepliesViewModel(result));
    }

    /// <summary>
    /// Creates a new root post in a specific thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread to post to.</param>
    /// <param name="request">The post creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created post.</returns>
    [HttpPost("threads/{threadId}/posts")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointName("createPost")]
    public async Task<IActionResult> CreatePost(
        string threadId,
        [FromBody] CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var coreRequest = new PostContributionRequest(request.Content, null);
            var result = await discourseApiClient
                .PostContributionToThreadAsync(threadId, coreRequest, cancellationToken);

            return CreatedAtAction(
                nameof(GetPostsByThreadId),
                new { threadId }, result.ContributionId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new reply to a specific post.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="postId">The ID of the post to reply to.</param>
    /// <param name="request">The reply creation request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created reply.</returns>
    [HttpPost("threads/{threadId}/posts/{postId}/replies")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointName("createReply")]
    public async Task<IActionResult> CreateReply(
        string threadId,
        string postId,
        [FromBody] CreateReplyRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var coreRequest = new PostContributionRequest(request.Content, postId);
        var result = await discourseApiClient
            .PostContributionToThreadAsync(threadId, coreRequest, cancellationToken);

        return CreatedAtAction(
            nameof(GetPostReplies),
            new { threadId, postId }, result.ContributionId);
    }
}
