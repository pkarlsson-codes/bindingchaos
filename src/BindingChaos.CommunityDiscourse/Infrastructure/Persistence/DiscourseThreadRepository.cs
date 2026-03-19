using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.SharedKernel.Persistence;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CommunityDiscourse.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the DiscourseThread repository for event sourcing.
/// </summary>
internal sealed class DiscourseThreadRepository : MartenRepository<DiscourseThread, DiscourseThreadId>, IDiscourseThreadRepository
{
    /// <summary>
    /// Initializes a new instance of the DiscourseThreadRepository class.
    /// </summary>
    /// <param name="session">The Marten document session.</param>
    /// <param name="logger">The logger instance.</param>
    public DiscourseThreadRepository(
        IDocumentSession session,
        ILogger<MartenRepository<DiscourseThread, DiscourseThreadId>> logger)
        : base(session, logger)
    {
    }
}