using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.SharedKernel.Domain;
using Neo4j.Driver;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// Neo4j-backed implementation of <see cref="ITrustGraphQueryService"/>.
/// </summary>
public sealed class Neo4jTrustGraphQueryService : ITrustGraphQueryService
{
    private readonly IDriver _driver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jTrustGraphQueryService"/> class.
    /// </summary>
    /// <param name="driver">The Neo4j driver.</param>
    public Neo4jTrustGraphQueryService(IDriver driver) => _driver = driver;

    /// <inheritdoc />
    public Task<IReadOnlySet<ParticipantId>> GetTrustedParticipantsAsync(
        ParticipantId participantId, int maxDegree, CancellationToken ct)
        => throw new NotImplementedException();
}
