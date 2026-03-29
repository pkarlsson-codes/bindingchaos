using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Ideas;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the Idea repository.
/// </summary>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger for the repository.</param>
internal sealed class IdeaRepository(
    IDocumentSession session,
    ILogger<MartenRepository<Idea, IdeaId>> logger)
    : MartenRepository<Idea, IdeaId>(session, logger), IIdeaRepository
{
}