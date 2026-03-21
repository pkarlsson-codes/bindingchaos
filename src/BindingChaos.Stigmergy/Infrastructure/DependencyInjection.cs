using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.Stigmergy.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Stigmergy bounded context.
/// </summary>
public static class StigmergyServiceCollectionExtensions
{
    /// <summary>
    /// Adds Stigmergy services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddStigmergy(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
