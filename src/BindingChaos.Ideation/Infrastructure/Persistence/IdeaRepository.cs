using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Persistence;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Ideation.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the Idea repository for event sourcing.
/// </summary>
internal class IdeaRepository : MartenRepository<Idea, IdeaId>, IIdeaRepository
{
    /// <summary>
    /// Initializes a new instance of the IdeaRepository class.
    /// </summary>
    /// <param name="session">The Marten document session.</param>
    /// <param name="logger">The logger instance.</param>
    public IdeaRepository(
        IDocumentSession session,
        ILogger<MartenRepository<Idea, IdeaId>> logger)
        : base(session, logger)
    {
    }
}
