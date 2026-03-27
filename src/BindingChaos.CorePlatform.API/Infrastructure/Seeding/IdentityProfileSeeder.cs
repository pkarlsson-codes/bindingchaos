using BindingChaos.IdentityProfile.Domain.Entities;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.CorePlatform.API.Infrastructure.Seeding;

/// <summary>
/// Seeds <see cref="Participant"/> records into the Identity Profile database on application startup.
/// Idempotent: skips participants that already exist.
/// </summary>
internal static class IdentityProfileSeeder
{
    /// <summary>
    /// Seeds participants from seed-participants.json into the Identity Profile database.
    /// </summary>
    /// <param name="services">The application service provider.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var profiles = SeedDataLoader.LoadParticipantProfiles();

        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityProfileDbContext>();

        var existingUserIds = await dbContext.Participants
            .AsNoTracking()
            .Select(p => p.UserId)
            .ToHashSetAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var profile in profiles)
        {
            if (existingUserIds.Contains(profile.UserId))
            {
                continue;
            }

            dbContext.Participants.Add(new Participant
            {
                UserId = profile.UserId,
                Pseudonym = profile.Pseudonym,
                CreatedAt = DateTimeOffset.UtcNow,
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
