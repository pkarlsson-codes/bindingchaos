using BindingChaos.Pseudonymity.Application.Services;
using BindingChaos.Pseudonymity.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.Pseudonymity.Infrastructure;

/// <summary>
/// DI registration for the Pseudonymity service.
/// </summary>
public static class PseudonymityServiceCollectionExtensions
{
    /// <summary>
    /// Adds Pseudonymity services to the container.
    /// </summary>
    public static IServiceCollection AddPseudonymity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PseudonymityConfiguration>(configuration.GetSection("Pseudonymity"));
        services.AddSingleton<IPseudonymService, PseudonymService>();
        return services;
    }
}


