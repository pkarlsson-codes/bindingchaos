using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.UserGroups;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.Stigmergy.Infrastructure.Projections;

/// <summary>
/// Projects user group events into <see cref="UserGroupListItemView"/>.
/// </summary>
internal sealed class UserGroupListItemViewProjection : SingleStreamProjection<UserGroupListItemView, string>
{
    /// <summary>
    /// Creates a new <see cref="UserGroupListItemView"/> from a <see cref="UserGroupFormed"/> event.
    /// </summary>
    /// <param name="e">The formed event.</param>
    /// <returns>A new <see cref="UserGroupListItemView"/>.</returns>
    public static UserGroupListItemView Create(IEvent<UserGroupFormed> e) =>
        new()
        {
            Id = e.Data.UserGroupId,
            CommonsId = e.Data.CommonsId,
            Name = e.Data.Name,
            Philosophy = e.Data.Philosophy,
            FounderId = e.Data.FounderId,
            FormedAt = e.Timestamp,
            MemberCount = 1,
            JoinPolicy = Enumeration<JoinPolicy>.FromValue(e.Data.Charter.MembershipRules.JoinPolicy).DisplayName,
        };

    /// <summary>
    /// Increments <see cref="UserGroupListItemView.MemberCount"/> when a member joins.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The joined event.</param>
    public static void Apply(UserGroupListItemView view, MemberJoined e)
    {
        view.MemberCount++;
    }

    /// <summary>
    /// Decrements <see cref="UserGroupListItemView.MemberCount"/> when a member leaves.
    /// </summary>
    /// <param name="view">The view to update.</param>
    /// <param name="e">The left event.</param>
    public static void Apply(UserGroupListItemView view, MemberLeft e)
    {
        view.MemberCount--;
    }
}
