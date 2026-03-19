using BindingChaos.Tagging.Application.ReadModels;
using Marten;

namespace BindingChaos.Tagging.Application.Queries;

/// <summary>
/// Query to retrieve global tags, optionally limited in count.
/// </summary>
public sealed record GetTags(int Limit);

/// <summary>
/// Handles retrieval of popular tags.
/// </summary>
public static class GetTagsHandler
{
    /// <summary>
    /// Retrieves the most frequently used tags.
    /// </summary>
    /// <param name="request">The query containing the limit.</param>
    /// <param name="querySession">The read-only query session for accessing the read model.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of <see cref="TagUsageView"/> representing the popular tags.</returns>
    public static async Task<TagUsageView[]> Handle(
        GetTags request,
        IQuerySession querySession,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tags = await querySession.Query<TagUsageView>()
            .Take(request.Limit > 0 ? request.Limit : 20)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return tags.ToArray();
    }
}
