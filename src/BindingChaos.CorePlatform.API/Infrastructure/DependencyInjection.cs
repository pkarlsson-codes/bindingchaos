using System.Text;
using BindingChaos.CommunityDiscourse.Infrastructure;
using BindingChaos.CommunityDiscourse.Infrastructure.Persistence;
using BindingChaos.CorePlatform.API.Infrastructure.DocumentManagement;
using BindingChaos.CorePlatform.API.Infrastructure.ExceptionHandling;
using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.IdentityProfile.Infrastructure;
using BindingChaos.Reputation.Infrastructure;
using BindingChaos.SharedKernel.Extensions;
using BindingChaos.Societies.Infrastructure;
using BindingChaos.Societies.Infrastructure.Persistence;
using BindingChaos.Stigmergy.Infrastructure;
using BindingChaos.Stigmergy.Infrastructure.Persistence;
using BindingChaos.Tagging.Infrastructure;
using BindingChaos.Tagging.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

namespace BindingChaos.CorePlatform.API.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Core Platform API.
/// </summary>
internal static class CorePlatformServiceCollectionExtensions
{
    /// <summary>
    /// Adds all Core Platform API services to the service collection in organized groups.
    /// This method orchestrates the registration of all services required by the Core Platform API.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="environment">The host environment instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddCorePlatformServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddCorePlatformCore();
        services.AddCorePlatformExceptionHandling();
        services.AddBoundedContexts(configuration);
        services.AddDocumentManagement(configuration);
        services.AddCorePlatformDatabase(configuration);
        services.AddCorePlatformHealthChecks();
        services.AddCorePlatformAuthentication(configuration, environment);
        services.AddCorePlatformAuthorization();

        return services;
    }

    private static IServiceCollection AddCorePlatformExceptionHandling(this IServiceCollection services)
    {
        // UnhandledExceptionHandler must be last — it catches everything.
        services.AddExceptionHandler<AggregateNotFoundExceptionHandler>();
        services.AddExceptionHandler<ForbiddenExceptionHandler>();
        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddExceptionHandler<UnhandledExceptionHandler>();
        return services;
    }

    private static IServiceCollection AddCorePlatformCore(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddIntegrationEventMappers(
            CommunityDiscourseAssemblyReference.Assembly,
            TaggingAssemblyReference.Assembly,
            StigmergyAssemblyReference.Assembly);
        services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
        services.AddFluentValidationAutoValidation();
        return services;
    }

    private static IServiceCollection AddBoundedContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTagging(configuration);
        services.AddIdentityProfile(configuration);
        services.AddCommunityDiscourse(configuration);
        services.AddSocieties(configuration);
        services.AddStigmergy(configuration);
        services.AddReputation(configuration);
        return services;
    }

    private static IServiceCollection AddCorePlatformDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                              throw new InvalidOperationException("Database connection string not found");

        services.AddMartenServices(
            connectionString,
            options =>
            {
                CommunityDiscourseMartenConfiguration.Configure(options);
                TaggingMartenConfiguration.Configure(options);
                SocietiesMartenConfiguration.Configure(options);
                StigmergyMartenConfiguration.Configure(options);
            });

        services.AddMartenUnitOfWork();
        return services;
    }

    private static IServiceCollection AddCorePlatformHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddIdentityProfileHealthChecks()
            .AddReputationHealthChecks();
        return services;
    }

    private static IServiceCollection AddCorePlatformAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var issuer = configuration["InternalJwt:Issuer"]
            ?? throw new InvalidOperationException("InternalJwt:Issuer is not configured.");
        var audience = configuration["InternalJwt:Audience"]
            ?? throw new InvalidOperationException("InternalJwt:Audience is not configured.");
        var secret = configuration["InternalJwt:SigningKey"]
                     ?? Environment.GetEnvironmentVariable("BINDINGCHAOS_INTERNAL_JWT_SECRET")
                     ?? throw new InvalidOperationException(
                         "Internal JWT signing key is not configured. Set InternalJwt:SigningKey or BINDINGCHAOS_INTERNAL_JWT_SECRET.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !environment.IsDevelopment();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "participant_id",
                };
            });
        return services;
    }

    private static IServiceCollection AddCorePlatformAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });
        return services;
    }
}
