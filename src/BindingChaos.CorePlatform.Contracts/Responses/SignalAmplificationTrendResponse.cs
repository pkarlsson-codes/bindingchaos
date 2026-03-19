namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for signal amplification trend data.
/// </summary>
public sealed record SignalAmplificationTrendResponse
{
    /// <summary>
    /// Gets the signal identifier.
    /// </summary>
    public string SignalId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the collection of trend data points showing amplification events over time.
    /// </summary>
    public IReadOnlyList<TrendPointResponse> DataPoints { get; init; } = Array.Empty<TrendPointResponse>();
}
