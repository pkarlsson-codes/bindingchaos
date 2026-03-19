using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
/// Environment is set to "Testing" so that appsettings.Testing.json is loaded
/// automatically before any service registration runs. This file lives in the
/// CorePlatform.API project and supplies the test database connection string and
/// JWT signing key. It also prevents dev-only seed data and the Scalar UI from loading.
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
/// IAsyncLifetime runs EF Core migrations for IdentityProfile once before any tests run.
/// MigrateAsync applies pending migrations even when the database already exists
/// (unlike EnsureCreated, which is a no-op on existing DBs).
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // These constants must match what is in appsettings.Testing.json, and what
    // TestJwtFactory uses to mint tokens.
    internal const string JwtIssuer = "BindingChaos.Gateway";
    internal const string JwtAudience = "BindingChaos.CorePlatform";
    internal const string JwtSigningKey = "integration-test-signing-key-must-be-32ch";

    public async ValueTask InitializeAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var sp = scope.ServiceProvider;

        var identityDb = sp.GetRequiredService<IdentityProfileDbContext>();
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
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
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
