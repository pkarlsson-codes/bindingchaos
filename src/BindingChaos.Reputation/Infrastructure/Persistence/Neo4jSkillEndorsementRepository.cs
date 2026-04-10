using BindingChaos.Reputation.Domain.SkillEndorsements;
using BindingChaos.SharedKernel.Domain;
using Neo4j.Driver;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// Neo4j-backed implementation of <see cref="ISkillEndorsementRepository"/>.
/// </summary>
public sealed class Neo4jSkillEndorsementRepository : ISkillEndorsementRepository
{
    private readonly IDriver _driver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jSkillEndorsementRepository"/> class.
    /// </summary>
    /// <param name="driver">The Neo4j driver.</param>
    public Neo4jSkillEndorsementRepository(IDriver driver) => _driver = driver;

    /// <inheritdoc />
    public async Task EndorseAsync(SkillEndorsement endorsement, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                """
                MERGE (a:Participant {id: $endorserId})
                MERGE (b:Participant {id: $endorseeId})
                MERGE (a)-[r:ENDORSED_FOR {skillId: $skillId}]->(b)
                ON CREATE SET r.grade = $grade, r.createdAt = $createdAt
                ON MATCH  SET r.grade = $grade, r.updatedAt = $updatedAt
                """,
                new
                {
                    endorserId = endorsement.EndorserId.Value,
                    endorseeId = endorsement.EndorseeId.Value,
                    skillId = endorsement.SkillId.ToString(),
                    grade = (int)endorsement.Grade,
                    createdAt = endorsement.CreatedAt.ToString("O"),
                    updatedAt = endorsement.UpdatedAt?.ToString("O"),
                }).ConfigureAwait(false);
            await cursor.ConsumeAsync().ConfigureAwait(false);
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task ReviseAsync(
        ParticipantId endorserId,
        ParticipantId endorseeId,
        Guid skillId,
        EndorsementGrade newGrade,
        DateTimeOffset updatedAt,
        CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                """
                MATCH (a:Participant {id: $endorserId})-[r:ENDORSED_FOR {skillId: $skillId}]->(b:Participant {id: $endorseeId})
                SET r.grade = $grade, r.updatedAt = $updatedAt
                """,
                new
                {
                    endorserId = endorserId.Value,
                    endorseeId = endorseeId.Value,
                    skillId = skillId.ToString(),
                    grade = (int)newGrade,
                    updatedAt = updatedAt.ToString("O"),
                }).ConfigureAwait(false);
            await cursor.ConsumeAsync().ConfigureAwait(false);
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task WithdrawAsync(ParticipantId endorserId, ParticipantId endorseeId, Guid skillId, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                "MATCH (a:Participant {id: $endorserId})-[r:ENDORSED_FOR {skillId: $skillId}]->(b:Participant {id: $endorseeId}) DELETE r",
                new
                {
                    endorserId = endorserId.Value,
                    endorseeId = endorseeId.Value,
                    skillId = skillId.ToString(),
                }).ConfigureAwait(false);
            await cursor.ConsumeAsync().ConfigureAwait(false);
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(ParticipantId endorserId, ParticipantId endorseeId, Guid skillId, CancellationToken ct)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                "MATCH (a:Participant {id: $endorserId})-[r:ENDORSED_FOR {skillId: $skillId}]->(b:Participant {id: $endorseeId}) RETURN count(r) > 0 AS exists",
                new
                {
                    endorserId = endorserId.Value,
                    endorseeId = endorseeId.Value,
                    skillId = skillId.ToString(),
                }).ConfigureAwait(false);
            var record = await cursor.SingleAsync().ConfigureAwait(false);
            return record["exists"].As<bool>();
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }
}
