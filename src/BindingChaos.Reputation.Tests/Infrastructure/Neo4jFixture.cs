using BindingChaos.Reputation.Infrastructure.Persistence;
using Neo4j.Driver;
using Testcontainers.Neo4j;

namespace BindingChaos.Reputation.Tests.Infrastructure;

public sealed class Neo4jFixture : IAsyncLifetime
{
    private Neo4jContainer _container = null!;

    public IDriver Driver { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _container = new Neo4jBuilder("neo4j:5")
            .WithEnvironment("NEO4J_AUTH", "none")
            .Build();

        await _container.StartAsync().ConfigureAwait(false);

        Driver = GraphDatabase.Driver(
            _container.GetConnectionString(),
            AuthTokens.None);

        var initializer = new Neo4jSchemaInitializer(Driver);
        await initializer.StartAsync(CancellationToken.None).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await Driver.DisposeAsync().ConfigureAwait(false);
        await _container.DisposeAsync().ConfigureAwait(false);
    }
}
