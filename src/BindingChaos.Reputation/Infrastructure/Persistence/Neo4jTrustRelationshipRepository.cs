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
    public async Task TrustAsync(TrustRelationship relationship, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                """
                MERGE (a:Participant {id: $trusterId})
                MERGE (b:Participant {id: $trusteeId})
                MERGE (a)-[r:TRUSTS]->(b)
                ON CREATE SET r.createdAt = $createdAt
                """,
                new
                {
                    trusterId = relationship.TrusterId.Value,
                    trusteeId = relationship.TrusteeId.Value,
                    createdAt = relationship.CreatedAt.ToString("O"),
                }).ConfigureAwait(false);
            await cursor.ConsumeAsync().ConfigureAwait(false);
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task WithdrawAsync(ParticipantId trusterId, ParticipantId trusteeId, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                "MATCH (a:Participant {id: $trusterId})-[r:TRUSTS]->(b:Participant {id: $trusteeId}) DELETE r",
                new { trusterId = trusterId.Value, trusteeId = trusteeId.Value })
                .ConfigureAwait(false);
            await cursor.ConsumeAsync().ConfigureAwait(false);
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(ParticipantId trusterId, ParticipantId trusteeId, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                "MATCH (a:Participant {id: $trusterId})-[r:TRUSTS]->(b:Participant {id: $trusteeId}) RETURN count(r) > 0 AS exists",
                new { trusterId = trusterId.Value, trusteeId = trusteeId.Value })
                .ConfigureAwait(false);
            var record = await cursor.SingleAsync().ConfigureAwait(false);
            return record["exists"].As<bool>();
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }
}
