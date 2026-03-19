namespace BindingChaos.SignalAwareness.Application.ReadModels;

/// <summary>
/// View used to project signal amplification trend information over time.
/// </summary>
public class SignalAmplificationTrendView
{
    /// <summary>
    /// The unique identifier for this trend view (same as SignalId).
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The signal identifier.
    /// </summary>
    public string SignalId { get; set; } = string.Empty;

    /// <summary>
    /// The collection of trend data points showing amplification events over time.
    /// </summary>
    public List<SignalAmplificationTrendPoint> DataPoints { get; set; } = [];
}
