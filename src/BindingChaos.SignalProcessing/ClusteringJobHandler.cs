using BindingChaos.Stigmergy.Contracts;
using Wolverine;

namespace BindingChaos.SignalProcessing;

/// <summary>
/// Handles <see cref="RunClusteringJob"/> by loading all stored embeddings, clustering them via the
/// Python sidecar, and publishing a <see cref="ClustersIdentifiedIntegrationEvent"/>.
/// </summary>
public sealed class ClusteringJobHandler(
    ISignalEmbeddingRepository embeddingRepository,
    IClusteringClient clusteringClient,
    IMessageBus bus)
{
    /// <summary>
    /// Runs the clustering job.
    /// </summary>
    /// <param name="message">The trigger message.</param>
    public async Task Handle(RunClusteringJob message)
    {
        var embeddings = await embeddingRepository.GetAllAsync().ConfigureAwait(false);

        if (embeddings.Count == 0)
            return;

        var results = await clusteringClient.ClusterAsync(embeddings).ConfigureAwait(false);

        var assignments = results
            .Select(r => new SignalClusterAssignment(r.SignalId, r.ClusterLabel))
            .ToList();

        await bus.PublishAsync(new ClustersIdentifiedIntegrationEvent(assignments)).ConfigureAwait(false);
    }
}
