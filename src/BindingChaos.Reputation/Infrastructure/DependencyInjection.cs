using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        throw new NotImplementedException();
    }
}
