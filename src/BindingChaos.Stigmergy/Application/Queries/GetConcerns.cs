using BindingChaos.Stigmergy.Application.ReadModels;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve all currently tracked concerns.</summary>
public sealed record GetConcerns;

/// <summary>Handles the <see cref="GetConcerns"/> query.</summary>
public static class GetConcernsHandler
{
    /// <summary>
    /// Returns all <see cref="ConcernsListItemView"/> documents ordered by concern label.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All concerns, ordered by concern label.</returns>
    public static async Task<IReadOnlyList<ConcernsListItemView>> Handle(
        GetConcerns request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await querySession
            .Query<ConcernsListItemView>()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}