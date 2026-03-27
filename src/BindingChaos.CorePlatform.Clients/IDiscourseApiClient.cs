using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client for interacting with the Discourse API.
/// </summary>
public interface IDiscourseApiClient
{
    /// <summary>
    /// Gets root contributions for a specific thread using cursor-based pagination.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of contributions to return.</param>
    /// <param name="direction">Direction of pagination.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contributions response.</returns>
    Task<GetContributionsResponse> GetContributionsByThreadIdAsync(
        string threadId,
        string? cursor = null,
        int limit = 5,
        CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets replies for a specific contribution using cursor-based pagination.
    /// </summary>
    /// <param name="contributionId">The ID of the contribution to get replies for.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of replies to return.</param>
    /// <param name="direction">Direction of pagination.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The replies response.</returns>
    Task<GetContributionRepliesResponse> GetContributionRepliesAsync(
        string contributionId,
        string? cursor = null,
        int limit = 5,
        CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Posts a new contribution to a specific thread.
    /// </summary>
    /// <param name="threadId">The ID of the thread to post to.</param>
    /// <param name="request">The contribution posting request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response containing the created contribution ID.</returns>
    Task<PostContributionResponse> PostContributionToThreadAsync(
        string threadId,
        PostContributionRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets contributions for a specific entity with cursor-based pagination.
    /// </summary>
    /// <param name="entityType">The type of entity (e.g., "idea", "signal", "action").</param>
    /// <param name="entityId">The ID of the entity.</param>
    /// <param name="cursor">Cursor for pagination (null for first page).</param>
    /// <param name="limit">Maximum number of contributions to return.</param>
    /// <param name="direction">Direction of pagination.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contributions response.</returns>
    Task<GetContributionsResponse> GetContributionsByEntityAsync(
        string entityType,
        string entityId,
        string? cursor = null,
        int limit = 5,
        CursorDirection direction = CursorDirection.Forward,
        CancellationToken cancellationToken = default);
}