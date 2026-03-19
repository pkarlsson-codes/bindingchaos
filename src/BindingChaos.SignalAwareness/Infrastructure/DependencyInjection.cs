using BindingChaos.SignalAwareness.Domain.Evidence;
using BindingChaos.SignalAwareness.Domain.Signals;
using BindingChaos.SignalAwareness.Domain.SuggestedActions;
using BindingChaos.SignalAwareness.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.SignalAwareness.Infrastructure;

/// <summary>
/// Dependency injection configuration for the SignalAwareness bounded context.
/// </summary>
public static class SignalAwarenessServiceCollectionExtensions
{
    /// <summary>
    /// Adds SignalAwareness services to the service collection.
    /// Note: Marten services should be registered at the application level, not here.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddSignalAwareness(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISignalRepository, SignalRepository>();
        services.AddScoped<IEvidenceRepository, EvidenceRepository>();
        services.AddScoped<ISignalActionRepository, SignalActionRepository>();
        return services;
    }
}
