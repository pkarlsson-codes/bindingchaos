namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Read model representing a single participant who has indicated affectedness on a concern.
/// </summary>
public sealed class ConcernAffectedParticipantsView
{
    /// <summary>
    /// Gets or sets the unique identifier for this record, composed as "{concernId}:{participantId}".
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the id of the concern.
    /// </summary>
    public string ConcernId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the id of the participant who indicated affectedness.
    /// </summary>
    public string ParticipantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when affectedness was indicated.
    /// </summary>
    public DateTimeOffset IndicatedAt { get; set; }
}
