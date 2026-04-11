using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Contracts;

/// <summary>
/// Published when the clustering job completes and cluster assignments are ready.
/// </summary>
/// <param name="Assignments">The cluster label assigned to each signal. A label of <c>-1</c> indicates noise (unclustered).</param>
public sealed record ClustersIdentifiedIntegrationEvent(
    IReadOnlyList<SignalClusterAssignment> Assignments
) : IntegrationEvent, IExternalIntegrationEvent;

/// <summary>
/// The cluster label assigned to a single signal.
/// </summary>
/// <param name="SignalId">Id of the signal.</param>
/// <param name="ClusterLabel">Cluster label; <c>-1</c> means noise/unclustered.</param>
/// <param name="Keywords">Top TF-IDF keywords for this signal's cluster. Empty for noise signals.</param>
public sealed record SignalClusterAssignment(string SignalId, int ClusterLabel, IReadOnlyList<string> Keywords);
