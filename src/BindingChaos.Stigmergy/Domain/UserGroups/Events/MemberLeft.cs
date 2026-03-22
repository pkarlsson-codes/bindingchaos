using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.UserGroups.Events;

/// <summary>
/// Represents an event indicating that a participant has left a user group.
/// </summary>
/// <param name="UserGroupId">The identifier of the user group.</param>
/// <param name="Version">The version of the event.</param>
/// <param name="ParticipantId">The identifier of the participant who left the group.</param>
public sealed record MemberLeft(string UserGroupId, long Version, string ParticipantId) : DomainEvent(UserGroupId, Version);
