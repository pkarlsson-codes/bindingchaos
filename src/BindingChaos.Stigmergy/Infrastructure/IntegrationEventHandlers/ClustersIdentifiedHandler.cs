using System.Globalization;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Contracts;
using Marten;

namespace BindingChaos.Stigmergy.Infrastructure.IntegrationEventHandlers;

/// <summary>
/// Handles <see cref="ClustersIdentifiedIntegrationEvent"/> by rebuilding the
/// <see cref="EmergingPatternView"/> documents: one per non-noise cluster.
/// </summary>
public static class ClustersIdentifiedHandler
{
    /// <summary>
    /// Replaces all <see cref="EmergingPatternView"/> documents with the latest cluster assignments.
    /// Noise signals (cluster label <c>-1</c>) are excluded.
    /// </summary>
    /// <param name="e">The clustering result event.</param>
    /// <param name="session">Marten document session.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        ClustersIdentifiedIntegrationEvent e,
        IDocumentSession session,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(e);

        session.DeleteWhere<EmergingPatternView>(_ => true);

        var now = DateTimeOffset.UtcNow;

        var patterns = e.Assignments
            .Where(a => a.ClusterLabel != -1)
            .GroupBy(a => a.ClusterLabel)
            .Select(g => new EmergingPatternView
            {
                Id = g.Key.ToString(CultureInfo.InvariantCulture),
                ClusterLabel = g.Key,
                SignalIds = [.. g.Select(a => a.SignalId)],
                SignalCount = g.Count(),
                LastUpdatedAt = now,
            });

        foreach (var pattern in patterns)
        {
            session.Store(pattern);
        }

        await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
