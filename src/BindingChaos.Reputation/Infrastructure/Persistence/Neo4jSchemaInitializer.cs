using Microsoft.Extensions.Hosting;
using Neo4j.Driver;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// Applies the Neo4j schema constraints at application startup.
/// </summary>
public sealed class Neo4jSchemaInitializer : IHostedService
{
    private readonly IDriver _driver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jSchemaInitializer"/> class.
    /// </summary>
    /// <param name="driver">The Neo4j driver.</param>
    public Neo4jSchemaInitializer(IDriver driver) => _driver = driver;

    /// <inheritdoc />
    /// <remarks>
    /// Applies:
    /// <list type="bullet">
    /// <item>Uniqueness constraint: FOR (p:Participant) REQUIRE p.id IS UNIQUE.</item>
    /// <item>Index on ENDORSED_FOR.skillId for efficient skill-based expert queries.</item>
    /// </list>
    /// </remarks>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(
                "CREATE CONSTRAINT participant_id_unique IF NOT EXISTS FOR (p:Participant) REQUIRE p.id IS UNIQUE")
                .ConfigureAwait(false);
            await cursor.ConsumeAsync().ConfigureAwait(false);

            cursor = await session.RunAsync(
                "CREATE INDEX endorsed_for_skill_idx IF NOT EXISTS FOR ()-[r:ENDORSED_FOR]-() ON (r.skillId)")
                .ConfigureAwait(false);
            await cursor.ConsumeAsync().ConfigureAwait(false);
        }
        finally
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
