using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.IdentityProfile.Infrastructure;

/// <summary>
/// DI registration for the Identity Profile module.
/// </summary>
public static class IdentityProfileServiceCollectionExtensions
{
    /// <summary>
    /// Adds Identity Profile services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddIdentityProfile(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new InvalidOperationException("IdentityProfile connection string not found");

        services.AddDbContext<IdentityProfileDbContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly(typeof(IdentityProfileDbContext).Assembly.FullName)));

        services.AddScoped<IIdentityProfileService, IdentityProfileService>();
        services.AddScoped<IPseudonymLookupService, PseudonymLookupService>();
        return services;
    }

    /// <summary>
    /// Adds Identity Profile health checks to the health check builder.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static IHealthChecksBuilder AddIdentityProfileHealthChecks(this IHealthChecksBuilder builder)
    {
        return builder.AddDbContextCheck<IdentityProfileDbContext>("identityprofile-db");
    }
}
