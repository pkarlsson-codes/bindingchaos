using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Societies.Domain.SocialContracts;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Societies.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the SocialContract repository for event sourcing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SocialContractRepository"/> class.
/// </remarks>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger instance.</param>
internal sealed class SocialContractRepository(
    IDocumentSession session,
    ILogger<MartenRepository<SocialContract, SocialContractId>> logger)
    : MartenRepository<SocialContract, SocialContractId>(session, logger), ISocialContractRepository
{
}
