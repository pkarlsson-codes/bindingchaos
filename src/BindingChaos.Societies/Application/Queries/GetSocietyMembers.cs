using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Societies.Application.ReadModels;
using BindingChaos.Societies.Domain.Societies;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Societies.Application.Queries;

/// <summary>
/// Query to retrieve a paginated list of active members for a society.
/// </summary>
/// <param name="SocietyId">The ID of the society.</param>
/// <param name="QuerySpec">The pagination specification.</param>
public sealed record GetSocietyMembers(SocietyId SocietyId, PaginationQuerySpec<EmptyFilter> QuerySpec);

/// <summary>
/// Handles the <see cref="GetSocietyMembers"/> query.
/// </summary>
public static class GetSocietyMembersHandler
{
    /// <summary>
    /// Returns a paginated list of active members for the specified society.
    /// </summary>
    /// <param name="request">The query containing the society ID and pagination spec.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of society member views.</returns>
    public static async Task<PaginatedResponse<SocietyMemberView>> Handle(
        GetSocietyMembers request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var page = await querySession.Query<SocietyMemberView>()
            .Where(m => m.SocietyId == request.SocietyId.Value && m.IsActive)
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<SocietyMemberView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}
