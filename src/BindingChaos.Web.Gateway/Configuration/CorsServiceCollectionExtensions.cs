namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// CORS registration helpers.
/// </summary>
public static class CorsServiceCollectionExtensions
{
    /// <summary>
    /// Adds a default CORS policy based on configuration, with a provided fallback list.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="defaultAllowedOrigins">Fallback origins when config is absent.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCorsFromConfig(this IServiceCollection services, IConfiguration configuration, string[] defaultAllowedOrigins)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? defaultAllowedOrigins;
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        return services;
    }
}
