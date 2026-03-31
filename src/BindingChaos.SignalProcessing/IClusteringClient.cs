namespace BindingChaos.SignalProcessing;

/// <summary>
/// Performs UMAP + HDBSCAN clustering over signal embeddings via the clustering sidecar.
/// </summary>
public interface IClusteringClient
{
    /// <summary>
    /// Submits signal embeddings for clustering and returns the label assigned to each signal.
    /// A label of <c>-1</c> indicates noise (unclustered).
    /// </summary>
    /// <param name="embeddings">The signal embeddings to cluster.</param>
    Task<IReadOnlyList<SignalClusterResult>> ClusterAsync(IReadOnlyList<SignalEmbedding> embeddings);
}

/// <summary>
/// The cluster assignment returned by the clustering sidecar for a single signal.
/// </summary>
/// <param name="SignalId">The signal identifier.</param>
/// <param name="ClusterLabel">The assigned cluster label; <c>-1</c> means noise/unclustered.</param>
/// <param name="Keywords">Top TF-IDF keywords for this signal's cluster. Empty for noise signals.</param>
public sealed record SignalClusterResult(string SignalId, int ClusterLabel, IReadOnlyList<string> Keywords);
