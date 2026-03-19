using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Domain.Signals;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.SignalAwareness.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the Signal repository for event sourcing.
/// </summary>
internal class SignalRepository : MartenRepository<Signal, SignalId>, ISignalRepository
{
    /// <summary>
    /// Initializes a new instance of the SignalRepository class.
    /// </summary>
    /// <param name="session">The Marten document session.</param>
    /// <param name="logger">The logger instance.</param>
    public SignalRepository(
        IDocumentSession session,
        ILogger<MartenRepository<Signal, SignalId>> logger)
        : base(session, logger)
    {
    }
}