using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;
using Marten;
using Marten.Events.Projections;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projection for the <see cref="UserGroupMembersView"/> read model.
/// Creates one document per membership to support paged queries.
/// </summary>
internal sealed class UserGroupMembersViewProjection : EventProjection
{
    /// <summary>
    /// Creates a new <see cref="UserGroupMembersView"/> when a member joins a user group.
    /// </summary>
    /// <param name="e">The event representing a member joining a user group.</param>
    /// <returns>A new <see cref="UserGroupMembersView"/> instance.</returns>
    public static UserGroupMembersView Create(MemberJoined e) => new()
    {
        Id = $"{e.AggregateId}:{e.ParticipantId}",
        UserGroupId = e.AggregateId,
        ParticipantId = e.ParticipantId,
        JoinedAt = e.OccurredAt,
    };

    /// <summary>
    /// Deletes the <see cref="UserGroupMembersView"/> when a member leaves a user group.
    /// </summary>
    /// <param name="e">The event representing a member leaving a user group.</param>
    /// <param name="ops">The document operations.</param>
    public static void Project(MemberLeft e, IDocumentOperations ops)
        => ops.Delete<UserGroupMembersView>($"{e.AggregateId}:{e.ParticipantId}");
}
