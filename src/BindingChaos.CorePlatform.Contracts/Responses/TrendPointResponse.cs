namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Generic response model for a single data point in any trend.
/// </summary>
public sealed record TrendPointResponse
{
    /// <summary>
    /// Gets the date and time of this trend point in ISO 8601 format.
    /// </summary>
    public string Date { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of event that occurred at this point in time.
    /// The specific values depend on the context (e.g., "support", "oppose", "amplify", "attenuate").
    /// </summary>
    public string EventType { get; init; } = string.Empty;
}
