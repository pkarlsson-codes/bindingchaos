using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.Ideation.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.Ideation.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Ideation bounded context.
/// </summary>
public static class IdeationServiceCollectionExtensions
{
    /// <summary>
    /// Adds Ideation services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddIdeation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IIdeaRepository, IdeaRepository>();
        services.AddScoped<IAmendmentRepository, AmendmentRepository>();
        return services;
    }
}
