namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Read model representing a single amplification of a signal.
/// </summary>
public sealed class SignalAmplificationsView
{
    /// <summary>
    /// Gets or sets the unique identifier for this amplification, composed as "{signalId}:{actorId}".
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the id of the signal that was amplified.
    /// </summary>
    public string SignalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the id of the actor that amplified the signal.
    /// </summary>
    public string ActorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the amplification occurred.
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }
}
