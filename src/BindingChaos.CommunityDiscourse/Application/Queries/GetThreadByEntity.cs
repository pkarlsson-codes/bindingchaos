using BindingChaos.CommunityDiscourse.Application.ReadModels;
using Marten;

namespace BindingChaos.CommunityDiscourse.Application.Queries;

/// <summary>
/// Query to get a discourse thread by entity reference.
/// </summary>
/// <param name="EntityType">The type of entity (e.g., "idea", "signal", "action").</param>
/// <param name="EntityId">The ID of the entity.</param>
public sealed record GetThreadByEntity(string EntityType, string EntityId);

/// <summary>
/// Handler for getting a discourse thread by entity reference.
/// </summary>
public static class GetThreadByEntityHandler
{
    /// <summary>
    /// Retrieves a discourse thread view based on the specified entity type and entity ID.
    /// </summary>
    /// <param name="request">The request containing the entity type and entity ID used to identify the thread.</param>
    /// <param name="querySession">The read-only query session for accessing the read model.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="DiscourseThreadView"/> if found; otherwise, <see langword="null"/>.</returns>
    public static async Task<DiscourseThreadView?> Handle(
        GetThreadByEntity request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(request.EntityType);
        ArgumentException.ThrowIfNullOrEmpty(request.EntityId);

        var normalizedEntityType = request.EntityType.ToLowerInvariant();

        return await querySession.Query<DiscourseThreadView>()
            .Where(t => t.EntityType == normalizedEntityType && t.EntityId == request.EntityId)
            .OrderByDescending(t => t.TotalContributions)
            .ThenByDescending(t => t.LastActivityAt)
            .ThenByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
