using BindingChaos.CommunityDiscourse.Infrastructure.Seeding;
using BindingChaos.Ideation.Infrastructure.Seeding;
using BindingChaos.SignalAwareness.Infrastructure.Seeding;
using BindingChaos.Societies.Infrastructure.Seeding;
using Marten;
using Marten.Schema;
using Minio;

namespace BindingChaos.CorePlatform.API.Infrastructure.Seeding;

/// <summary>
/// Runs development seed data against the Marten document store.
/// Must be called after the host has fully started so that Wolverine is ready to receive events.
/// </summary>
internal static class DevelopmentSeeder
{
    /// <summary>
    /// Seeds development data into Marten. Each seeder is idempotent and skips if data already exists.
    /// </summary>
    /// <param name="services">The application service provider.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var minio = services.GetRequiredService<IMinioClient>();
        await DocumentSeeder.SeedAsync(minio, cancellationToken).ConfigureAwait(false);

        var store = services.GetRequiredService<IDocumentStore>();
        var participants = SeedDataLoader.LoadParticipants();

        IInitialData[] seeders =
        [
            new SignalAwarenessInitialData(participants),
            new SocietiesInitialData(participants),
            new IdeationInitialData(participants),
            new CommunityDiscourseInitialData(participants),
        ];

        foreach (var seeder in seeders)
        {
            await seeder.Populate(store, cancellationToken).ConfigureAwait(false);
        }
    }
}
