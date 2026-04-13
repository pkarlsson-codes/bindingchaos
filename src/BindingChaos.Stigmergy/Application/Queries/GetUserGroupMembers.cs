using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve a paginated list of members for a user group.
/// </summary>
/// <param name="UserGroupId">The user group identifier.</param>
/// <param name="CallerId">The identifier of the requesting participant, or <see langword="null"/> for anonymous.</param>
/// <param name="QuerySpec">Pagination and sort parameters.</param>
public sealed record GetUserGroupMembers(UserGroupId UserGroupId, ParticipantId? CallerId, PaginationQuerySpec QuerySpec);

/// <summary>
/// Handles the <see cref="GetUserGroupMembers"/> query.
/// </summary>
public static class GetUserGroupMembersHandler
{
    /// <summary>
    /// Returns a paginated list of member pseudonyms for the specified user group.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="userGroupRepository">Repository for loading the user group aggregate.</param>
    /// <param name="pseudonymLookupService">Service for resolving participant pseudonyms.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A paginated list of members, or <see langword="null"/> if the group does not exist.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the member list is private and the caller is not a member.
    /// </exception>
    public static async Task<PaginatedResponse<UserGroupMemberView>?> Handle(
        GetUserGroupMembers request,
        IQuerySession querySession,
        IUserGroupRepository userGroupRepository,
        IPseudonymLookupService pseudonymLookupService,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var aggregate = await userGroupRepository.GetByIdAsync(request.UserGroupId, cancellationToken)
            .ConfigureAwait(false);

        if (aggregate is null)
        {
            return null;
        }

        var listView = await querySession
            .LoadAsync<UserGroupListItemView>(request.UserGroupId.Value, cancellationToken)
            .ConfigureAwait(false);

        var isMember = request.CallerId is not null
            && listView is not null
            && listView.MemberParticipantIds.Contains(request.CallerId.Value);

        if (!aggregate.Charter.MembershipRules.MemberListPublic && !isMember)
        {
            throw new UnauthorizedAccessException("Member list is private.");
        }

        var page = await querySession.Query<UserGroupMembersView>()
            .Where(v => v.UserGroupId == request.UserGroupId.Value)
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        var participantIds = page.Select(v => v.ParticipantId).ToList();
        var pseudonyms = await pseudonymLookupService
            .GetPseudonymsAsync(participantIds, cancellationToken)
            .ConfigureAwait(false);

        var items = page
            .Select(v => new UserGroupMemberView(
                pseudonyms.GetValueOrDefault(v.ParticipantId, "Anonymous")))
            .ToArray();

        return new PaginatedResponse<UserGroupMemberView>
        {
            Items = items,
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}
