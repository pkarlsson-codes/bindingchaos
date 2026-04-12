using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve detailed information about a user group.
/// </summary>
/// <param name="UserGroupId">The user group identifier.</param>
/// <param name="CallerId">The identifier of the requesting participant, if authenticated.</param>
public sealed record GetUserGroupDetail(UserGroupId UserGroupId, ParticipantId? CallerId);

/// <summary>
/// Handles the <see cref="GetUserGroupDetail"/> query.
/// </summary>
public static class GetUserGroupDetailHandler
{
    /// <summary>
    /// Returns detailed information about a user group, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="userGroupRepository">Repository for loading the user group aggregate.</param>
    /// <param name="pseudonymLookupService">Service for resolving participant pseudonyms.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user group detail view, or <see langword="null"/> if not found.</returns>
    public static async Task<UserGroupDetailView?> Handle(
        GetUserGroupDetail request,
        IQuerySession querySession,
        IUserGroupRepository userGroupRepository,
        IPseudonymLookupService pseudonymLookupService,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var listView = await querySession.Query<UserGroupListItemView>()
            .Where(v => v.Id == request.UserGroupId.Value)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (listView is null)
        {
            return null;
        }

        var userGroup = await userGroupRepository.GetByIdOrThrowAsync(request.UserGroupId, cancellationToken)
            .ConfigureAwait(false);

        var commonsView = await querySession.Query<CommonsListItemView>()
            .Where(v => v.Id == listView.CommonsId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        var founderPseudonym = await pseudonymLookupService
            .GetPseudonymAsync(listView.FounderId, cancellationToken)
            .ConfigureAwait(false);

        var isMember = request.CallerId != null && listView.MemberParticipantIds.Contains(request.CallerId.Value);

        var approvalRules = userGroup.Charter.MembershipRules.ApprovalRules;
        var approvalSettings = approvalRules is null
            ? null
            : new UserGroupApprovalSettingsView(
                (double)approvalRules.ApprovalThreshold,
                approvalRules.VetoEnabled);

        return new UserGroupDetailView(
            listView.Id,
            listView.CommonsId,
            commonsView?.Name ?? "Unknown",
            listView.Name,
            listView.Philosophy,
            founderPseudonym ?? "Anonymous",
            listView.FormedAt,
            listView.MemberCount,
            listView.JoinPolicy,
            isMember,
            new UserGroupCharterView(
                new UserGroupMembershipRulesView(
                    userGroup.Charter.MembershipRules.JoinPolicy.DisplayName,
                    userGroup.Charter.MembershipRules.MaxMembers,
                    userGroup.Charter.MembershipRules.EntryRequirements,
                    userGroup.Charter.MembershipRules.MemberListPublic,
                    approvalSettings),
                new UserGroupContestationRulesView(
                    userGroup.Charter.ContentionRules.ResolutionWindow.ToString(),
                    (double)userGroup.Charter.ContentionRules.RejectionThreshold),
                new UserGroupShunningRulesView(
                    (double)userGroup.Charter.ShunningRules.ApprovalThreshold)));
    }
}
