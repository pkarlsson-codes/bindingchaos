using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Represents a participant's membership within a <see cref="UserGroup"/>.
/// </summary>
internal sealed class Membership : Entity<MembershipId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Membership"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for this membership.</param>
    /// <param name="participantId">The identifier of the participant who holds this membership.</param>
    /// <param name="joinedAt">The timestamp when the participant joined the group.</param>
    internal Membership(MembershipId id, ParticipantId participantId, DateTimeOffset joinedAt)
    {
        Id = id;
        ParticipantId = participantId;
        JoinedAt = joinedAt;
    }

    /// <summary>
    /// Gets the identifier of the participant who holds this membership.
    /// </summary>
    public ParticipantId ParticipantId { get; }

    /// <summary>
    /// Gets the timestamp when the participant joined the group.
    /// </summary>
    public DateTimeOffset JoinedAt { get; }
}
