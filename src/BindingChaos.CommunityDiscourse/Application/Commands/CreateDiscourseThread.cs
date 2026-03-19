using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.SharedKernel.Persistence;
using Marten;

namespace BindingChaos.CommunityDiscourse.Application.Commands;

/// <summary>
/// Command to create a new contribution thread.
/// </summary>
public sealed record CreateDiscourseThread(string EntityType, string EntityId);

/// <summary>
/// Handles the creation of a new contribution thread in the discourse system.
/// </summary>
public static class CreateDiscourseThreadHandler
{
    /// <summary>
    /// Creates a new discourse thread for the specified entity, or returns the ID of an existing one.
    /// </summary>
    /// <param name="request">The command containing the entity type and ID to create a thread for.</param>
    /// <param name="querySession">The read-only query session for checking existing threads.</param>
    /// <param name="discourseThreadRepository">The repository for discourse thread aggregates.</param>
    /// <param name="unitOfWork">The unit of work for persisting changes.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created or existing discourse thread.</returns>
    public static async Task<DiscourseThreadId> Handle(
        CreateDiscourseThread request,
        IQuerySession querySession,
        IDiscourseThreadRepository discourseThreadRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(request.EntityType);
        ArgumentException.ThrowIfNullOrEmpty(request.EntityId);

        var normalizedEntityType = request.EntityType.ToLowerInvariant();

        var existingThread = await querySession.Query<DiscourseThreadView>()
            .Where(t => t.EntityType == normalizedEntityType && t.EntityId == request.EntityId)
            .OrderByDescending(t => t.TotalContributions)
            .ThenByDescending(t => t.LastActivityAt)
            .ThenByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (existingThread != null)
        {
            return DiscourseThreadId.Create(existingThread.Id);
        }

        var thread = DiscourseThread.Create(EntityReference.Create(normalizedEntityType, request.EntityId));
        discourseThreadRepository.Stage(thread);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return thread.Id;
    }
}
