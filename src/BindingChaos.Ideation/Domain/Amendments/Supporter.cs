using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Amendments;

/// <summary>
/// Represents a participant's support for an amendment with their reasoning.
/// </summary>
public sealed class Supporter : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Supporter"/> value object.
    /// </summary>
    /// <param name="participantId">The unique identifier of the participant supporting the amendment.</param>
    /// <param name="reason">The participant's reason or commentary for supporting the amendment.</param>
    /// <exception cref="ArgumentException">Thrown if the reason is null, empty, or exceeds the maximum length.</exception>
    public Supporter(ParticipantId participantId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason cannot be null or empty", nameof(reason));
        }

        if (reason.Length > 1000)
        {
            throw new ArgumentException("Reason cannot exceed 1000 characters", nameof(reason));
        }

        ParticipantId = participantId;
        Reason = reason.Trim();
    }

    /// <summary>
    /// Gets the unique identifier of the participant supporting the amendment.
    /// </summary>
    public ParticipantId ParticipantId { get; }

    /// <summary>
    /// Gets the participant's reason or commentary for supporting the amendment.
    /// </summary>
    public string Reason { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ParticipantId;
        yield return Reason;
    }
}
