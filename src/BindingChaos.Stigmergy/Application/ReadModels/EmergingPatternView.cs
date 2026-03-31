namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Read model representing a cluster of signals that form an emerging pattern.
/// Rebuilt on every <see cref="Stigmergy.Contracts.ClustersIdentifiedIntegrationEvent"/>.
/// </summary>
public sealed class EmergingPatternView
{
    /// <summary>
    /// Stable document identity derived from the cluster label (e.g. "0", "1").
    /// </summary>
    required public string Id { get; set; }

    /// <summary>
    /// The HDBSCAN cluster label assigned to this pattern.
    /// </summary>
    required public int ClusterLabel { get; set; }

    /// <summary>
    /// IDs of the signals assigned to this cluster.
    /// </summary>
    required public List<string> SignalIds { get; set; }

    /// <summary>
    /// Number of signals in this cluster.
    /// </summary>
    required public int SignalCount { get; set; }

    /// <summary>
    /// When this pattern was last updated by a clustering run.
    /// </summary>
    required public DateTimeOffset LastUpdatedAt { get; set; }

    /// <summary>
    /// Top TF-IDF keywords extracted by the clustering sidecar for this cluster.
    /// </summary>
    required public List<string> Keywords { get; set; }
}
