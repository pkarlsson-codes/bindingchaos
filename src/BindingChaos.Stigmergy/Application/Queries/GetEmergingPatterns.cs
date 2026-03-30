using BindingChaos.Stigmergy.Application.ReadModels;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve all currently identified emerging patterns.
/// </summary>
public sealed record GetEmergingPatterns;

/// <summary>Handles the <see cref="GetEmergingPatterns"/> query.</summary>
public static class GetEmergingPatternsHandler
{
    /// <summary>
    /// Returns all <see cref="EmergingPatternView"/> documents ordered by cluster label.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All emerging patterns, ordered by cluster label.</returns>
    public static async Task<IReadOnlyList<EmergingPatternView>> Handle(
        GetEmergingPatterns request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await querySession
            .Query<EmergingPatternView>()
            .OrderBy(x => x.ClusterLabel)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
