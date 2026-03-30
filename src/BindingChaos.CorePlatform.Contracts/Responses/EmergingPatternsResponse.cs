namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Feed of emerging signal patterns identified by the clustering pipeline.
/// </summary>
public sealed record EmergingPatternsResponse
{
    /// <summary>
    /// The identified emerging patterns, ordered by cluster label.
    /// </summary>
    public IReadOnlyList<EmergingPatternResponse> Patterns { get; init; } = [];
}
