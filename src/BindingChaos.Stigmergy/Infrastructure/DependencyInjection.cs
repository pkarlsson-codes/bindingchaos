using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.Ideas;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using BindingChaos.Stigmergy.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BindingChaos.Stigmergy.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Stigmergy bounded context.
/// </summary>
public static class StigmergyServiceCollectionExtensions
{
    /// <summary>
    /// Adds Stigmergy services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddStigmergy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICommonsRepository, CommonsRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IIdeaRepository, IdeaRepository>();
        return services;
    }
}
