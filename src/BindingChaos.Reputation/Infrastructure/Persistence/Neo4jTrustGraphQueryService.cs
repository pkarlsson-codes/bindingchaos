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
    public async Task<IReadOnlySet<ParticipantId>> GetTrustedParticipantsAsync(
        ParticipantId participantId, int maxDegree, CancellationToken ct)
    {
        var clamped = Math.Clamp(maxDegree, 1, 5);
        var query = $"MATCH (start:Participant {{id: $participantId}})-[:TRUSTS*1..{clamped}]->(trusted:Participant) RETURN DISTINCT trusted.id AS id";

        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(query, new { participantId = participantId.Value })
                .ConfigureAwait(false);
            var records = await cursor.ToListAsync(ct).ConfigureAwait(false);
            return records.Select(r => new ParticipantId(r["id"].As<string>())).ToHashSet();
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }
}
