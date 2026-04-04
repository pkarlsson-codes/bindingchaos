using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve all user groups governing the specified commons.</summary>
/// <param name="CommonsId">The commons to filter by.</param>
/// <param name="QuerySpec">Pagination and sorting parameters.</param>
public sealed record GetUserGroupsForCommons(CommonsId CommonsId, PaginationQuerySpec QuerySpec);

/// <summary>Handles the <see cref="GetUserGroupsForCommons"/> query.</summary>
public static class GetUserGroupsForCommonsHandler
{
    /// <summary>
    /// Returns all <see cref="UserGroupListItemView"/> documents for the given commons, ordered by the requested sort.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of user groups.</returns>
    public static async Task<PaginatedResponse<UserGroupListItemView>> Handle(
        GetUserGroupsForCommons request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = querySession.Query<UserGroupListItemView>()
            .Where(v => v.CommonsId == request.CommonsId.Value)
            .Sort(request.QuerySpec.SortDescriptors, UserGroupListItemView.SortMappings);

        var page = await query
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<UserGroupListItemView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}
