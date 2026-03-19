namespace BindingChaos.SignalAwareness.Application.ReadModels;

/// <summary>
/// Represents a single amplification event in the signal amplification trend.
/// </summary>
public class SignalAmplificationTrendPoint
{
    /// <summary>
    /// The date and time of this amplification event.
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// The type of amplification event: "amplify" or "attenuate".
    /// </summary>
    public string EventType { get; set; } = string.Empty;
}
