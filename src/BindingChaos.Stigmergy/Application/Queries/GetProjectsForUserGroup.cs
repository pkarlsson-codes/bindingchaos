using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve projects for a given user group.
/// </summary>
/// <param name="UserGroupId">The user group owning the projects.</param>
/// <param name="QuerySpec">Pagination and sort details.</param>
public sealed record GetProjectsForUserGroup(UserGroupId UserGroupId, PaginationQuerySpec QuerySpec);

/// <summary>
/// Handles the <see cref="GetProjectsForUserGroup"/> query.
/// </summary>
public static class GetProjectsForUserGroupHandler
{
    /// <summary>
    /// Returns a paginated list of projects owned by the specified user group.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of projects.</returns>
    public static async Task<PaginatedResponse<ProjectsListItemView>> Handle(
        GetProjectsForUserGroup request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = querySession.Query<ProjectsListItemView>()
            .Where(v => v.UserGroupId == request.UserGroupId.Value)
            .Sort(request.QuerySpec.SortDescriptors, ProjectsListItemView.SortMappings);

        var page = await query
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<ProjectsListItemView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}
