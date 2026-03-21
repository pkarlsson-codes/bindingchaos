using BindingChaos.CorePlatform.API.Services;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using Wolverine;

namespace BindingChaos.CorePlatform.API.IntegrationTests;

/// <summary>Wolverine extension that switches durability off for integration tests.</summary>
internal sealed class MediatorOnlyExtension : IWolverineExtension
{
    /// <inheritdoc/>
    public void Configure(WolverineOptions options) =>
        options.Durability.Mode = DurabilityMode.MediatorOnly;
}

/// <summary>
/// Boots the real CorePlatform API in-process for integration tests.
///
/// A PostgreSQL Testcontainer is started in InitializeAsync and its connection string
/// is injected before the host builds, so no external database is needed.
///
/// Minio is replaced with a no-op IDocumentManagementService — tests that exercise
/// document management will need to revisit this when the time comes.
///
/// All async projections are switched to Inline so that projections are applied
/// synchronously within the same transaction as the events. Queries are immediately
/// consistent and no background daemon is needed.
///
/// Wolverine is set to MediatorOnly mode. This disables the PostgreSQL-backed durable
/// inbox/outbox and the DurableReceiver background service. Without this, handler
/// exceptions cause Wolverine to attempt ReleaseIncomingAsync on teardown, which gets
/// stuck in a retry loop against a cancelled CancellationToken and hangs the process.
///
/// IAsyncLifetime runs the PostgreSQL container and EF Core migrations for IdentityProfile
/// once before any tests run.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // These constants must match what is in appsettings.Testing.json, and what
    // TestJwtFactory uses to mint tokens.
    internal const string JwtIssuer = "BindingChaos.Gateway";
    internal const string JwtAudience = "BindingChaos.CorePlatform";
    internal const string JwtSigningKey = "integration-test-signing-key-must-be-32ch";

    private readonly PostgreSqlContainer _pgContainer = BuildPostgresContainer();

    private static PostgreSqlContainer BuildPostgresContainer()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Testing.json")
            .Build();

        return new PostgreSqlBuilder()
            .WithDatabase(config["TestDatabase:Database"])
            .WithUsername(config["TestDatabase:Username"])
            .WithPassword(config["TestDatabase:Password"])
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        // Start the container before the host builds so ConfigureWebHost can inject
        // the connection string.
        await _pgContainer.StartAsync().ConfigureAwait(false);

        await using var scope = Services.CreateAsyncScope();
        var identityDb = scope.ServiceProvider.GetRequiredService<IdentityProfileDbContext>();
        await identityDb.Database.MigrateAsync().ConfigureAwait(false);
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            await base.DisposeAsync().ConfigureAwait(false);
        }
        catch
        {
            // Suppress exceptions thrown during host shutdown. Hosted services (Marten,
            // Wolverine) can throw when stopped after the test run completes. All tests
            // have already passed at this point — letting these propagate causes xUnit to
            // report a collection cleanup failure and return exit code 1 despite no test
            // failures.
        }

        await _pgContainer.DisposeAsync().ConfigureAwait(false);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var containerConnStr = _pgContainer.GetConnectionString();

            // Both AddMartenServices and AddIdentityProfile capture the connection string by
            // value at service registration time — ConfigureAppConfiguration cannot reach them
            // after the fact. We override the live connections here instead.

            // Redirect Marten to the container. ConfigureMarten is applied when IDocumentStore
            // is first resolved, after all registrations, so this wins over the original.
            services.ConfigureMarten(opts => opts.Connection(containerConnStr));

            // Redirect the EF Core DbContext to the container.
            services.RemoveAll<DbContextOptions<IdentityProfileDbContext>>();
            services.AddDbContext<IdentityProfileDbContext>(options =>
                options.UseNpgsql(containerConnStr,
                    b => b.MigrationsAssembly(typeof(IdentityProfileDbContext).Assembly.FullName)));

            // Replace Minio-backed document management with a no-op.
            services.RemoveAll<IDocumentManagementService>();
            services.AddScoped<IDocumentManagementService, NullDocumentManagementService>();

            // Switch all async projections to inline so they are applied synchronously
            // within the same transaction as the events. No daemon required, and queries
            // are immediately consistent — which is what we need in tests.
            //
            // ISubscriptionSource.Lifecycle is read-only at the interface level, but
            // the concrete projection classes (ProjectionBase) do expose a settable Lifecycle.
            services.ConfigureMarten(opts =>
            {
                foreach (var source in opts.Projections.All.OfType<ProjectionBase>())
                    source.Lifecycle = ProjectionLifecycle.Inline;
            });

            // Disable Wolverine's durable inbox/outbox. With IntegrateWithWolverine() active,
            // handler exceptions cause Wolverine to enqueue a retry via ReleaseIncomingAsync.
            // On test teardown that call races against a cancelled CancellationToken and the
            // retry loop in DurableReceiver never exits, hanging the process indefinitely.
            // MediatorOnly removes all node persistence so no background receiver runs.
            services.AddWolverineExtension<MediatorOnlyExtension>();
        });
    }
}
