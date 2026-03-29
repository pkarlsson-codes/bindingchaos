namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Amplification data point view.
/// </summary>
public class AmplificationTrendView
{
    /// <summary>
    /// The ID of the signal that this amplification trend is associated with.
    /// </summary>
    required public string SignalId { get; set; }

    /// <summary>
    /// The collection of amplification data points that make up the trend.
    /// </summary>
    required public List<AmplificationDataPoint> DataPoints { get; set; }

    /// <summary>
    /// Represents a single amplification data point in the trend.
    /// </summary>
    public class AmplificationDataPoint
    {
        /// <summary>
        /// The timestamp of the data point.
        /// </summary>
        required public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Indicates whether the data point represents an amplification or withdrawal event.
        /// </summary>
        required public bool IsAmplified { get; set; }
    }

}