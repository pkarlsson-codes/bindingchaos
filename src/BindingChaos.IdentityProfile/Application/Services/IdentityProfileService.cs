using BindingChaos.IdentityProfile.Domain;
using BindingChaos.IdentityProfile.Domain.Entities;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using BindingChaos.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BindingChaos.IdentityProfile.Application.Services;

/// <summary>
/// Implementation for identity mapping operations.
/// </summary>
public sealed class IdentityProfileService(IdentityProfileDbContext dbContext) : IIdentityProfileService
{
    private const int MaxPseudonymAttempts = 10;
    private const string PostgresUniqueViolationCode = "23505";

    /// <inheritdoc />
    public async Task<string> LinkOrGetUserIdAsync(string provider, string subject, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        var existingUserId = await dbContext.IdentityMaps
            .AsNoTracking()
            .Where(x => x.Provider == provider && x.Sub == subject)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!string.IsNullOrEmpty(existingUserId))
            return existingUserId;

        var userId = ParticipantId.Generate().Value;
        dbContext.IdentityMaps.Add(new IdentityMap
        {
            Provider = provider,
            Sub = subject,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        for (var attempt = 0; attempt < MaxPseudonymAttempts; attempt++)
        {
            var candidate = PseudonymGenerator.Generate(Random.Shared);
            var taken = await dbContext.Participants
                .AnyAsync(p => p.Pseudonym == candidate, cancellationToken)
                .ConfigureAwait(false);
            if (taken)
                continue;

            var participant = new Participant
            {
                UserId = userId,
                Pseudonym = candidate,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            dbContext.Participants.Add(participant);

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return userId;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == PostgresUniqueViolationCode)
            {
                dbContext.Entry(participant).State = EntityState.Detached;
            }
        }

        throw new InvalidOperationException($"Could not generate unique pseudonym after {MaxPseudonymAttempts} attempts.");
    }

    /// <inheritdoc />
    public async Task<IdentityMap?> GetIdentityMapAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return await dbContext.IdentityMaps.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken).ConfigureAwait(false);
    }
}
