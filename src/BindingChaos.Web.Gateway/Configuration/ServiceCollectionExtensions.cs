using BindingChaos.CorePlatform.Clients;
using BindingChaos.Web.Gateway.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Extension methods for registering Core Platform API clients in the Web Gateway.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Core Platform API clients to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when CorePlatform:BaseAddress configuration is missing.</exception>
    public static IServiceCollection AddCorePlatformClients(this IServiceCollection services, ConfigurationManager configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var corePlatformBaseAddress = configuration["CorePlatform:BaseAddress"]
            ?? throw new InvalidOperationException("CorePlatform:BaseAddress configuration is required");

        services.AddHttpContextAccessor();
        services.AddSingleton<IInternalJwtService, InternalJwtService>();
        services.TryAddScoped<InternalGatewayAuthHandler>();

        services.AddTransient<InternalServiceTokenHandler>();
        services.AddHttpClient(HttpClientNames.CorePlatformSystem, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CorePlatformOptions>>();
            client.BaseAddress = new Uri(options.Value.BaseAddress ?? corePlatformBaseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }).AddHttpMessageHandler<InternalServiceTokenHandler>();

        services.AddSignalsApiClient(corePlatformBaseAddress)
            .AddHttpMessageHandler<InternalGatewayAuthHandler>();
        services.AddIdeasApiClient(corePlatformBaseAddress)
            .AddHttpMessageHandler<InternalGatewayAuthHandler>();
        services.AddAmendmentsApiClient(corePlatformBaseAddress)
            .AddHttpMessageHandler<InternalGatewayAuthHandler>();
        services.AddDiscourseApiClient(corePlatformBaseAddress)
            .AddHttpMessageHandler<InternalGatewayAuthHandler>();
        services.AddDocumentsApiClient(corePlatformBaseAddress)
            .AddHttpMessageHandler<InternalGatewayAuthHandler>();
        services.AddTagsApiClient(corePlatformBaseAddress)
            .AddHttpMessageHandler<InternalGatewayAuthHandler>();
        services.AddSocietiesApiClient(corePlatformBaseAddress)
            .AddHttpMessageHandler<InternalGatewayAuthHandler>();
        services.AddInviteLinksApiClient(corePlatformBaseAddress)
            .AddHttpMessageHandler<InternalGatewayAuthHandler>();

        return services;
    }
}
