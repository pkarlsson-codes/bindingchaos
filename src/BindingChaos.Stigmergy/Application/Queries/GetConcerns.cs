using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Stigmergy.Application.ReadModels;
using Marten;
using Marten.Linq;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve concerns with optional filtering and pagination.</summary>
public sealed record GetConcerns(PaginationQuerySpec<ConcernsQueryFilter> QuerySpec);

/// <summary>Handles the <see cref="GetConcerns"/> query.</summary>
public static class GetConcernsHandler
{
    /// <summary>
    /// Returns a paginated list of concerns matching the query criteria.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of concerns matching the query criteria.</returns>
    public static async Task<PaginatedResponse<ConcernsListItemView>> Handle(
        GetConcerns request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var filter = request.QuerySpec.Filter;

        var baseQuery = querySession.Query<ConcernsListItemView>();

        var filtered = string.IsNullOrWhiteSpace(filter.RaisedByParticipantId)
            ? baseQuery
            : (IMartenQueryable<ConcernsListItemView>)baseQuery.Where(c => c.RaisedById == filter.RaisedByParticipantId);

        var sortedQuery = filtered.Sort(request.QuerySpec.SortDescriptors, ConcernsListItemView.SortMappings);

        var page = await sortedQuery
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