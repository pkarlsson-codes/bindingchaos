using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.UserGroups.Events;

/// <summary>
/// Represents an event indicating that a participant has joined a user group.
/// </summary>
/// <param name="UserGroupId">The identifier of the user group.</param>
/// <param name="Version">The version of the event.</param>
/// <param name="MembershipId">The identifier of the membership created for the participant who joined the group.</param>
/// <param name="ParticipantId">The identifier of the participant who joined the group.</param>
public sealed record MemberJoined(string UserGroupId, long Version, string MembershipId, string ParticipantId) : DomainEvent(UserGroupId, Version);