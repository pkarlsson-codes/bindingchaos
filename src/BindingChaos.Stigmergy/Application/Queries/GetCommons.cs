using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Stigmergy.Application.ReadModels;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve all currently tracked commons.</summary>
public sealed record GetCommons(PaginationQuerySpec QuerySpec);

/// <summary>Handles the <see cref="GetCommons"/> query.</summary>
public static class GetCommonsHandler
{
    /// <summary>
    /// Returns all <see cref="CommonsListItemView"/> documents ordered by the requested sort.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of commons.</returns>
    public static async Task<PaginatedResponse<CommonsListItemView>> Handle(
        GetCommons request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = querySession.Query<CommonsListItemView>()
            .Sort(request.QuerySpec.SortDescriptors, CommonsListItemView.SortMappings);

        var page = await query
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<CommonsListItemView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}
