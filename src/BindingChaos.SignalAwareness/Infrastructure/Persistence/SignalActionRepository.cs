using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Domain.SuggestedActions;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.SignalAwareness.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the <see cref="SuggestedAction"/> repository.
/// </summary>
internal class SignalActionRepository : MartenRepository<SuggestedAction, SuggestedActionId>, ISignalActionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SignalActionRepository"/> class.
    /// </summary>
    /// <param name="session">The Marten document session.</param>
    /// <param name="logger">The logger instance.</param>
    public SignalActionRepository(
        IDocumentSession session,
        ILogger<MartenRepository<SuggestedAction, SuggestedActionId>> logger)
        : base(session, logger)
    {
    }
}