using BindingChaos.CommunityDiscourse.Domain.Contributions;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.CommunityDiscourse.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.CommunityDiscourse.Infrastructure;

/// <summary>
/// Extension methods for registering CommunityDiscourse bounded context services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds CommunityDiscourse bounded context services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddCommunityDiscourse(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddScoped<IDiscourseThreadRepository, DiscourseThreadRepository>();
        services.AddScoped<IContributionRepository, ContributionRepository>();

        return services;
    }
}
