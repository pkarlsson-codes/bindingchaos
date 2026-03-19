using BindingChaos.Web.Gateway.Services;
using StackExchange.Redis;

namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Registration helpers for server-side token storage backed by Redis.
/// </summary>
public static class TokenStoreServiceCollectionExtensions
{
    /// <summary>
    /// Registers a Redis-backed <see cref="ITokenStore"/> using configuration key
    /// <c>Authentication:TokenStore:Redis:ConnectionString</c>.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the Redis connection string is missing.</exception>
    public static IServiceCollection AddTokenStoreFromConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetValue<string>("Authentication:TokenStore:Redis:ConnectionString");
        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            throw new InvalidOperationException("Redis connection string is required for token store (Authentication:TokenStore:Redis:ConnectionString)");
        }

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddSingleton<ITokenStore, RedisTokenStore>();
        return services;
    }
}
