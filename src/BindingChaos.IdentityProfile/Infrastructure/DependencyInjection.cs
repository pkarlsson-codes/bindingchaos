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
    public static IServiceCollection AddIdentityProfile(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new InvalidOperationException("IdentityProfile connection string not found");

        services.AddDbContext<IdentityProfileDbContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly(typeof(IdentityProfileDbContext).Assembly.FullName)));

        services.AddScoped<IIdentityProfileService, IdentityProfileService>();
        return services;
    }

    public static IHealthChecksBuilder AddIdentityProfileHealthChecks(this IHealthChecksBuilder builder)
    {
        return builder.AddDbContextCheck<IdentityProfileDbContext>("identityprofile-db");
    }
}


