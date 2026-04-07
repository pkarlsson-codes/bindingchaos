using System.Reflection;
using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.CorePlatform.API.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering integration event mappers in the service collection.
/// </summary>
internal static class IntegrationEventMapperRegistration
{
    /// <summary>
    /// Registers integration event mappers from the specified assemblies into the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the mappers to.</param>
    /// <param name="assemblies">The assemblies to scan for integration event mappers. If null or empty, the executing assembly is used.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddIntegrationEventMappers(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies is null || assemblies.Length == 0)
        {
            assemblies = [Assembly.GetExecutingAssembly()];
        }

        foreach (var asm in assemblies)
        {
            var types = asm.DefinedTypes.Where(t => !t.IsAbstract && !t.IsInterface).ToList();

            foreach (var t in types)
            {
                var mapperIfaces = t.ImplementedInterfaces
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventMapper<>));

                foreach (var iface in mapperIfaces)
                {
                    services.AddSingleton(iface, t.AsType());
                }
            }
        }

        return services;
    }
}
