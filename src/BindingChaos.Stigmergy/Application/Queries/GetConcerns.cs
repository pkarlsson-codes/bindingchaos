using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Stigmergy.Application.ReadModels;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve all currently tracked concerns.</summary>
public sealed record GetConcerns(PaginationQuerySpec QuerySpec);

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
    public static async Task<PaginatedResponse<ConcernsListItemView>> Handle(
        GetConcerns request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = querySession.Query<ConcernsListItemView>()
            .Sort(request.QuerySpec.SortDescriptors, ConcernsListItemView.SortMappings);

        var page = await query
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<ConcernsListItemView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}