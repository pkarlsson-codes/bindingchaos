using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Concerns;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the <see cref="Concern"/> repository.
/// </summary>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger for the repository.</param>
internal sealed class ConcernRepository(
    IDocumentSession session,
    ILogger<MartenRepository<Concern, ConcernId>> logger)
    : MartenRepository<Concern, ConcernId>(session, logger), IConcernRepository
{
}