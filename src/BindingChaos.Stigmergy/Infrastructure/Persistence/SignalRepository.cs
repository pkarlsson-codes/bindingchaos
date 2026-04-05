using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Signals;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the <see cref="Signal"/> repository.
/// </summary>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger for the repository.</param>
internal sealed class SignalRepository(
    IDocumentSession session,
    ILogger<MartenRepository<Signal, SignalId>> logger)
    : MartenRepository<Signal, SignalId>(session, logger), ISignalRepository
{
}