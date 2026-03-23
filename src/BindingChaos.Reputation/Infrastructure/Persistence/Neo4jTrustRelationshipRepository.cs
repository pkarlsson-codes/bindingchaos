using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.SharedKernel.Domain;
using Neo4j.Driver;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// Neo4j-backed implementation of <see cref="ITrustRelationshipRepository"/>.
/// </summary>
public sealed class Neo4jTrustRelationshipRepository : ITrustRelationshipRepository
{
    private readonly IDriver _driver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jTrustRelationshipRepository"/> class.
    /// </summary>
    /// <param name="driver">The Neo4j driver.</param>
    public Neo4jTrustRelationshipRepository(IDriver driver) => _driver = driver;

    /// <inheritdoc />
    public Task TrustAsync(TrustRelationship relationship, CancellationToken ct)
        => throw new NotImplementedException();

    /// <inheritdoc />
    public Task WithdrawAsync(ParticipantId trusterId, ParticipantId trusteeId, CancellationToken ct)
        => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<bool> ExistsAsync(ParticipantId trusterId, ParticipantId trusteeId, CancellationToken ct)
        => throw new NotImplementedException();
}
