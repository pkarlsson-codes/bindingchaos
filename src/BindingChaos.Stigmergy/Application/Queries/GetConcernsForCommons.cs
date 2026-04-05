using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve all concerns linked to the specified commons.</summary>
/// <param name="CommonsId">The commons to retrieve linked concerns for.</param>
public sealed record GetConcernsForCommons(CommonsId CommonsId);

/// <summary>Handles the <see cref="GetConcernsForCommons"/> query.</summary>
public static class GetConcernsForCommonsHandler
{
    /// <summary>
    /// Returns all <see cref="ConcernsListItemView"/> documents linked to the given commons.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of concerns linked to the commons.</returns>
    public static async Task<ConcernsListItemView[]> Handle(
        GetConcernsForCommons request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var commonsView = await querySession.LoadAsync<CommonsListItemView>(request.CommonsId.Value, cancellationToken).ConfigureAwait(false);

        if (commonsView is null || commonsView.LinkedConcernIds.Count == 0)
        {
            return [];
        }

        var concerns = await querySession.Query<ConcernsListItemView>()
            .Where(v => commonsView.LinkedConcernIds.Contains(v.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return [.. concerns];
    }
}
