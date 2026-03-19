using BindingChaos.SharedKernel.Persistence;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Marten;

namespace BindingChaos.SharedKernel.Extensions;

/// <summary>
/// Extension methods for configuring Marten services in the dependency injection container.
/// </summary>
public static class MartenServiceCollectionExtensions
{
    /// <summary>
    /// Adds Marten services to the service collection with the specified connection string.
    /// This method ensures Marten services are only registered once per application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    /// <param name="configureOptions">Optional configuration action for Marten options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <param name="configureBuilder">Optional configuration action for the Marten builder (e.g., registering IInitialData seeders).</param>
    public static IServiceCollection AddMartenServices(
        this IServiceCollection services,
        string connectionString,
        Action<StoreOptions>? configureOptions = null,
        Action<Marten.MartenServiceCollectionExtensions.MartenConfigurationExpression>? configureBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        if (services.Any(x => x.ServiceType == typeof(IDocumentStore)))
        {
            throw new InvalidOperationException("Marten services are already registered.");
        }

        var marten = services.AddMarten(opts =>
        {
            opts.Connection(connectionString);
            opts.Events.StreamIdentity = StreamIdentity.AsString;
            opts.Events.DatabaseSchemaName = "bindingchaos_events";
            configureOptions?.Invoke(opts);
        })
            .UseLightweightSessions()
            .IntegrateWithWolverine()
            .PublishEventsToWolverine("everything"); // TODO: Implement scoped topics for different bounded contexts

        marten.AddAsyncDaemon(DaemonMode.Solo);
        marten.ApplyAllDatabaseChangesOnStartup();

        configureBuilder?.Invoke(marten);

        return services;
    }

    /// <summary>
    /// Adds the Marten unit of work to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMartenUnitOfWork(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IUnitOfWork, MartenUnitOfWork>();

        return services;
    }
}
