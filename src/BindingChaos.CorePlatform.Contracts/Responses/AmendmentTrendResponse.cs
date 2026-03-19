namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for amendment support trend data.
/// </summary>
public sealed record AmendmentTrendResponse
{
    /// <summary>
    /// Gets the amendment identifier.
    /// </summary>
    public string AmendmentId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the collection of trend data points showing supporter/opponent counts over time.
    /// </summary>
    public IReadOnlyList<TrendPointResponse> DataPoints { get; init; } = Array.Empty<TrendPointResponse>();
}
