using BindingChaos.Reputation.Domain.SkillEndorsements;
using BindingChaos.SharedKernel.Domain;
using Neo4j.Driver;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// Neo4j-backed implementation of <see cref="ICompetenceGraphQueryService"/>.
/// </summary>
public sealed class Neo4jCompetenceGraphQueryService : ICompetenceGraphQueryService
{
    private readonly IDriver _driver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jCompetenceGraphQueryService"/> class.
    /// </summary>
    /// <param name="driver">The Neo4j driver.</param>
    public Neo4jCompetenceGraphQueryService(IDriver driver) => _driver = driver;

    /// <inheritdoc />
    public async Task<IReadOnlyList<SkillExpertResult>> GetSkillExpertsAsync(Guid skillId, int limit, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                """
                MATCH (endorser:Participant)-[e:ENDORSED_FOR {skillId: $skillId}]->(expert:Participant)
                RETURN expert.id AS participantId,
                       count(endorser)          AS endorsementCount,
                       avg(toFloat(e.grade))    AS averageGrade,
                       sum(e.grade)             AS totalScore
                ORDER BY totalScore DESC
                LIMIT $limit
                """,
                new { skillId = skillId.ToString(), limit }).ConfigureAwait(false);

            var records = await cursor.ToListAsync(ct).ConfigureAwait(false);
            return records.Select(r => new SkillExpertResult(
                new ParticipantId(r["participantId"].As<string>()),
                r["endorsementCount"].As<int>(),
                r["averageGrade"].As<double>(),
                r["totalScore"].As<int>())).ToList();
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CompetencyResult>> GetParticipantCompetenciesAsync(
        ParticipantId participantId, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                """
                MATCH (endorser:Participant)-[e:ENDORSED_FOR]->(p:Participant {id: $participantId})
                RETURN e.skillId                AS skillId,
                       count(endorser)          AS endorsementCount,
                       avg(toFloat(e.grade))    AS averageGrade
                ORDER BY endorsementCount DESC
                """,
                new { participantId = participantId.Value }).ConfigureAwait(false);

            var records = await cursor.ToListAsync(ct).ConfigureAwait(false);
            return records.Select(r => new CompetencyResult(
                Guid.Parse(r["skillId"].As<string>()),
                r["endorsementCount"].As<int>(),
                r["averageGrade"].As<double>())).ToList();
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SkillExpertResult>> GetTrustWeightedExpertsAsync(
        ParticipantId viewerId, Guid skillId, int limit, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                """
                MATCH (viewer:Participant {id: $viewerId})-[:TRUSTS*1..3]->(trusted:Participant)
                MATCH (trusted)-[e:ENDORSED_FOR {skillId: $skillId}]->(expert:Participant)
                RETURN expert.id                        AS participantId,
                       count(DISTINCT trusted)          AS endorsementCount,
                       avg(toFloat(e.grade))            AS averageGrade,
                       sum(e.grade)                     AS totalScore
                ORDER BY totalScore DESC
                LIMIT $limit
                """,
                new { viewerId = viewerId.Value, skillId = skillId.ToString(), limit }).ConfigureAwait(false);

            var records = await cursor.ToListAsync(ct).ConfigureAwait(false);
            return records.Select(r => new SkillExpertResult(
                new ParticipantId(r["participantId"].As<string>()),
                r["endorsementCount"].As<int>(),
                r["averageGrade"].As<double>(),
                r["totalScore"].As<int>())).ToList();
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }
}
