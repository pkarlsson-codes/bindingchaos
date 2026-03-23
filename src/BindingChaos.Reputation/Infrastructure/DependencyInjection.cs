using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.Reputation.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace BindingChaos.Reputation.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Reputation bounded context.
/// </summary>
public static class ReputationServiceCollectionExtensions
{
    /// <summary>
    /// Adds Reputation services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddReputation(this IServiceCollection services, IConfiguration configuration)
    {
        var neo4jUri = configuration["Neo4j:Uri"]
            ?? throw new InvalidOperationException("Neo4j:Uri is not configured.");

        services.AddSingleton<IDriver>(_ => GraphDatabase.Driver(neo4jUri));
        services.AddScoped<ITrustRelationshipRepository, Neo4jTrustRelationshipRepository>();
        services.AddScoped<ITrustGraphQueryService, Neo4jTrustGraphQueryService>();
        services.AddHostedService<Neo4jSchemaInitializer>();
        return services;
    }
}
