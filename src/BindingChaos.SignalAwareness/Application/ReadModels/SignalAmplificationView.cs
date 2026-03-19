namespace BindingChaos.SignalAwareness.Application.ReadModels;

/// <summary>
/// Read model entity for signal amplifications optimized for querying.
/// This is populated from EventStoreDB events via projections.
/// </summary>
public class SignalAmplificationView
{
    /// <summary>
    /// The unique identifier for the signal amplification.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the signal being amplified.
    /// </summary>
    public string SignalId { get; set; } = string.Empty;

    /// <summary>
    /// The pseudonym of the participant who amplified the signal.
    /// </summary>
    public string AmplifierId { get; set; } = string.Empty;

    /// <summary>
    /// The reason for amplifying the signal.
    /// </summary>
    public int Reason { get; set; }

    /// <summary>
    /// Optional commentary provided with the amplification.
    /// </summary>
    public string? Commentary { get; set; }

    /// <summary>
    /// When the signal was amplified.
    /// </summary>
    public DateTimeOffset AmplifiedAt { get; set; }

    /// <summary>
    /// Whether this amplification is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The version number for optimistic concurrency control.
    /// </summary>
    public long Version { get; set; }

    /// <summary>
    /// Navigation property to the associated signal.
    /// </summary>
    public virtual SignalView Signal { get; set; } = null!;
}