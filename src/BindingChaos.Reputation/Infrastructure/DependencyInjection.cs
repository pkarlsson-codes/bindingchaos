using BindingChaos.Reputation.Domain.SkillDomains;
using BindingChaos.Reputation.Domain.SkillEndorsements;
using BindingChaos.Reputation.Domain.Skills;
using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.Reputation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Reputation connection string 'Database' is not configured.");

        services.AddSingleton<IDriver>(_ => GraphDatabase.Driver(neo4jUri));

        services.AddDbContext<CompetenceDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(CompetenceDbContext).Assembly.FullName)));

        services.AddScoped<ITrustRelationshipRepository, Neo4jTrustRelationshipRepository>();
        services.AddScoped<ITrustGraphQueryService, Neo4jTrustGraphQueryService>();
        services.AddScoped<ISkillDomainRepository, EfCoreSkillDomainRepository>();
        services.AddScoped<ISkillRepository, EfCoreSkillRepository>();
        services.AddScoped<ISkillEndorsementRepository, Neo4jSkillEndorsementRepository>();
        services.AddScoped<ICompetenceGraphQueryService, Neo4jCompetenceGraphQueryService>();

        services.AddHostedService<Neo4jSchemaInitializer>();
        return services;
    }

    /// <summary>
    /// Adds Reputation health checks to the health checks builder.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <returns>The health checks builder for method chaining.</returns>
    public static IHealthChecksBuilder AddReputationHealthChecks(this IHealthChecksBuilder builder)
    {
        return builder.AddDbContextCheck<CompetenceDbContext>("reputation-competence-db");
    }
}
