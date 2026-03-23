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
    public Task StartAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
