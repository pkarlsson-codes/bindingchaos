using BindingChaos.Stigmergy.Domain.UserGroups;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Read model representing a single member of a user group.
/// </summary>
public sealed class UserGroupMembersView
{
    /// <summary>
    /// Gets or sets the unique identifier for this membership, composed as "{userGroupId}:{participantId}".
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the id of the <see cref="UserGroup"/>.</summary>
    public string UserGroupId { get; set; } = string.Empty;

    /// <summary>Gets or sets the participant id of the member.</summary>
    public string ParticipantId { get; set; } = string.Empty;

    /// <summary>Gets or sets the time that the member joined the user group.</summary>
    public DateTimeOffset JoinedAt { get; set; }
}
