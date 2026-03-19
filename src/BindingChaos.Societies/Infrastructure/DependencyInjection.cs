using BindingChaos.Societies.Domain.SocialContracts;
using BindingChaos.Societies.Domain.Societies;
using BindingChaos.Societies.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.Societies.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Societies bounded context.
/// </summary>
public static class SocietiesServiceCollectionExtensions
{
    /// <summary>
    /// Adds Societies services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddSocieties(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISocietyRepository, SocietyRepository>();
        services.AddScoped<ISocialContractRepository, SocialContractRepository>();
        return services;
    }
}
