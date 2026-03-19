using BindingChaos.CommunityDiscourse.Domain.Contributions;
using BindingChaos.SharedKernel.Persistence;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CommunityDiscourse.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the Contribution repository for event sourcing.
/// </summary>
internal sealed class ContributionRepository : MartenRepository<Contribution, ContributionId>, IContributionRepository
{
    /// <summary>
    /// Initializes a new instance of the ContributionRepository class.
    /// </summary>
    /// <param name="session">The Marten document session.</param>
    /// <param name="logger">The logger instance.</param>
    public ContributionRepository(
        IDocumentSession session,
        ILogger<MartenRepository<Contribution, ContributionId>> logger)
        : base(session, logger)
    {
    }
}