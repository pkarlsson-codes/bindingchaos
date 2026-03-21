using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// A proposed amendment to a project, submitted by a participant for deliberation.
/// </summary>
public sealed class Amendment : Entity<AmendmentId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Amendment"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for this amendment.</param>
    /// <param name="proposedBy">The participant who proposed this amendment.</param>
    /// <param name="proposedAt">The timestamp when this amendment was proposed.</param>
    /// <param name="status">The initial status of this amendment.</param>
    internal Amendment(AmendmentId id, ParticipantId proposedBy, DateTimeOffset proposedAt, AmendmentStatus status)
    {
        Id = id;
        ProposedBy = proposedBy;
        ProposedAt = proposedAt;
        Status = status;
    }

    /// <summary>
    /// Gets the participant who proposed this amendment.
    /// </summary>
    public ParticipantId ProposedBy { get; }

    /// <summary>
    /// Gets the timestamp when this amendment was proposed.
    /// </summary>
    public DateTimeOffset ProposedAt { get; }

    /// <summary>
    /// Gets the current status of this amendment.
    /// </summary>
    public AmendmentStatus Status { get; internal set; }
}
