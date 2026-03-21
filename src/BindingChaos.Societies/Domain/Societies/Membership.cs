using BindingChaos.SharedKernel.Domain;

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
    public Membership(MembershipId id, ParticipantId participantId)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(participantId);
        Id = id;
        ParticipantId = participantId;
        IsActive = true;
    }

    /// <summary>
    /// Gets the participant who holds this membership.
    /// </summary>
    public ParticipantId ParticipantId { get; }

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
