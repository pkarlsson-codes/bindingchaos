namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Represents a cluster of signals forming an emerging pattern.
/// </summary>
public sealed record EmergingPatternResponse
{
    /// <summary>
    /// The HDBSCAN cluster label.
    /// </summary>
    public int ClusterLabel { get; init; }

    /// <summary>
    /// IDs of the signals assigned to this cluster.
    /// </summary>
    public IReadOnlyList<string> SignalIds { get; init; } = [];

    /// <summary>
    /// Number of signals in this cluster.
    /// </summary>
    public int SignalCount { get; init; }

    /// <summary>
    /// When this pattern was last updated by a clustering run.
    /// </summary>
    public DateTimeOffset LastUpdatedAt { get; init; }
}
