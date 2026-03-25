using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the Commons repository for event sourcing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the CommonsRepository class.
/// </remarks>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger instance.</param>
internal class CommonsRepository(
    IDocumentSession session,
    ILogger<MartenRepository<Commons, CommonsId>> logger)
    : MartenRepository<Commons, CommonsId>(session, logger), ICommonsRepository
{
}
