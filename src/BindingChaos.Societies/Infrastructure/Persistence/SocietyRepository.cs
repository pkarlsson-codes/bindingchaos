using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Societies.Domain.Societies;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Societies.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the Society repository for event sourcing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SocietyRepository"/> class.
/// </remarks>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger instance.</param>
internal sealed class SocietyRepository(
    IDocumentSession session,
    ILogger<MartenRepository<Society, SocietyId>> logger) : MartenRepository<Society, SocietyId>(session, logger), ISocietyRepository
{
}
