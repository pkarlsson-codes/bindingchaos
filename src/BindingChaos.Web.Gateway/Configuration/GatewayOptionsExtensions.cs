namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// General service registration helpers.
/// </summary>
public static class GatewayOptionsExtensions
{
    /// <summary>
    /// Binds strongly-typed options used across the gateway.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGatewayOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorePlatformOptions>(configuration.GetSection("CorePlatform"));
        services.Configure<GatewayOptions>(configuration.GetSection("Gateway"));
        services.Configure<TokenStoreOptions>(configuration.GetSection("Authentication:TokenStore"));
        return services;
    }
}
