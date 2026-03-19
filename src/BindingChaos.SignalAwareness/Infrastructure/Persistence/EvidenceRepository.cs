using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Domain.Evidence;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.SignalAwareness.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the Evidence repository.
/// </summary>
internal class EvidenceRepository : MartenRepository<Evidence, EvidenceId>, IEvidenceRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EvidenceRepository"/> class.
    /// </summary>
    /// <param name="session">The Marten document session.</param>
    /// <param name="logger">The logger instance.</param>
    public EvidenceRepository(
        IDocumentSession session,
        ILogger<MartenRepository<Evidence, EvidenceId>> logger)
        : base(session, logger)
    {
    }
}
