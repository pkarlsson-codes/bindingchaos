using BindingChaos.SharedKernel.Domain;
using BindingChaos.Societies.Domain.SocialContracts;

namespace BindingChaos.Societies.Domain.Societies;

/// <summary>
/// Represents a participant's membership in a society.
/// </summary>
public sealed class Membership : Entity<MembershipId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Membership"/> class.
    /// </summary>
    /// <param name="id">The membership identifier.</param>
    /// <param name="participantId">The participant who joined.</param>
    /// <param name="socialContractId">The social contract agreed to at join time.</param>
    /// <param name="joinedAt">The timestamp when the participant joined.</param>
    public Membership(MembershipId id, ParticipantId participantId, SocialContractId socialContractId, DateTimeOffset joinedAt)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(participantId);
        ArgumentNullException.ThrowIfNull(socialContractId);
        Id = id;
        ParticipantId = participantId;
        SocialContractId = socialContractId;
        JoinedAt = joinedAt;
        IsActive = true;
    }

    /// <summary>
    /// Gets the participant who holds this membership.
    /// </summary>
    public ParticipantId ParticipantId { get; }

    /// <summary>
    /// Gets the social contract agreed to when joining.
    /// </summary>
    public SocialContractId SocialContractId { get; }

    /// <summary>
    /// Gets the timestamp when the participant joined.
    /// </summary>
    public DateTimeOffset JoinedAt { get; }

    /// <summary>
    /// Gets a value indicating whether this membership is currently active.
    /// </summary>
    public bool IsActive { get; internal set; }

    /// <summary>
    /// Deactivates this membership.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}
