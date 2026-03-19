using BindingChaos.Tagging.Application.Services;
using BindingChaos.Tagging.Domain.TaggableTargets;
using BindingChaos.Tagging.Domain.Tags;
using BindingChaos.Tagging.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.Tagging.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Tagging bounded context.
/// </summary>
public static class TaggingServiceCollectionExtensions
{
    /// <summary>
    /// Adds Tagging services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddTagging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITagResolver, TagResolver>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ITaggableTargetRepository, TaggableTargetRepository>();
        return services;
    }
}