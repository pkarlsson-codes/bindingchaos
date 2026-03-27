namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Extension methods for configuring WebApplicationBuilder services in logical groups.
/// </summary>
internal static class GatewayServicesExtensions
{
    /// <summary>
    /// Registers all core Gateway services including MVC, API clients, and gateway options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGatewayServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddMvcWithQuerying();
        services.AddCorePlatformClients(configuration);
        services.AddGatewayOptions(configuration);

        return services;
    }

    /// <summary>
    /// Registers authentication and security services including cookies, OIDC, and token handling.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGatewayAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCookieAndOidcAuth(configuration);
        services.AddTokenStoreFromConfig(configuration);

        return services;
    }

    /// <summary>
    /// Registers infrastructure services like CORS, logging, and health checks.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGatewayInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddCorsFromConfig(configuration, GatewayDefaults.DevelopmentCorsOrigins);
        services.AddHealthChecks();
        return services;
    }

    /// <summary>
    /// Registers development-only services and tools including OpenAPI and in-memory services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGatewayDevelopmentServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOpenApiDocumentation(configuration);
    }

    /// <summary>
    /// Configures Serilog logging from configuration.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <returns>The host builder for chaining.</returns>
    public static IHostBuilder ConfigureGatewayLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilogFromConfig();
    }
}