using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// A pledge of resources made by a participant toward a project requirement.
/// </summary>
public sealed class Pledge : Entity<PledgeId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Pledge"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for this pledge.</param>
    /// <param name="pledgedBy">The participant making the pledge.</param>
    /// <param name="pledgedAt">The timestamp when the pledge was made.</param>
    /// <param name="amount">The quantity of the resource being pledged.</param>
    internal Pledge(PledgeId id, ParticipantId pledgedBy, DateTimeOffset pledgedAt, double amount)
    {
        Id = id;
        PledgedBy = pledgedBy;
        PledgedAt = pledgedAt;
        Amount = amount;
    }

    /// <summary>
    /// Gets the participant who made this pledge.
    /// </summary>
    public ParticipantId PledgedBy { get; }

    /// <summary>
    /// Gets the timestamp when this pledge was made.
    /// </summary>
    public DateTimeOffset PledgedAt { get; }

    /// <summary>
    /// Gets the quantity of the resource being pledged.
    /// </summary>
    public double Amount { get; }

    /// <summary>
    /// Gets a value indicating whether this pledge has been withdrawn.
    /// </summary>
    public bool IsWithdrawn { get; internal set; }
}
