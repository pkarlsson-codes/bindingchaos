using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.ReadModels;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve all user groups the specified participant is a member of.</summary>
/// <param name="ParticipantId">The participant whose memberships to look up.</param>
public sealed record GetMyUserGroups(ParticipantId ParticipantId);

/// <summary>Handles the <see cref="GetMyUserGroups"/> query.</summary>
public static class GetMyUserGroupsHandler
{
    /// <summary>
    /// Returns all <see cref="UserGroupListItemView"/> documents for which the participant is a member.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of user groups the participant belongs to.</returns>
    public static async Task<UserGroupListItemView[]> Handle(
        GetMyUserGroups request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var results = await querySession.Query<UserGroupListItemView>()
            .Where(v => v.MemberParticipantIds.Contains(request.ParticipantId.Value))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return [.. results];
    }
}
