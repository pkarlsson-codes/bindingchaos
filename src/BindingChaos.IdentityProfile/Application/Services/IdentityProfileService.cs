using BindingChaos.IdentityProfile.Domain.Entities;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using BindingChaos.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Application.Services;

/// <summary>
/// Implementation for identity mapping and trust operations.
/// </summary>
public sealed class IdentityProfileService(IdentityProfileDbContext dbContext) : IIdentityProfileService
{
    public async Task<string> LinkOrGetUserIdAsync(string provider, string subject, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        var existing = await dbContext.IdentityMaps
            .AsNoTracking()
            .Where(x => x.Provider == provider && x.Sub == subject)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!string.IsNullOrEmpty(existing))
        {
            return existing;
        }

        var userId = ParticipantId.Generate().Value;
        dbContext.IdentityMaps.Add(new IdentityMap
        {
            Provider = provider,
            Sub = subject,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        dbContext.UserTrusts.Add(new UserTrust
        {
            UserId = userId,
            PersonhoodVerified = false,
            TrustLevel = "unknown",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return userId;
    }

    public async Task<IdentityMap?> GetIdentityMapAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return await dbContext.IdentityMaps.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<UserTrust> SetPersonhoodAsync(string userId, bool verified, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        var row = await dbContext.UserTrusts.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken).ConfigureAwait(false);
        if (row is null)
        {
            row = new UserTrust
            {
                UserId = userId,
                PersonhoodVerified = verified,
                TrustLevel = verified ? "verified" : "unknown",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };
            dbContext.UserTrusts.Add(row);
        }
        else
        {
            row.PersonhoodVerified = verified;
            row.TrustLevel = verified ? "verified" : row.TrustLevel;
            row.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return row;
    }

    public async Task<UserTrust> GetOrCreateTrustAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        var row = await dbContext.UserTrusts.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken).ConfigureAwait(false);
        if (row is not null)
        {
            return row;
        }

        row = new UserTrust
        {
            UserId = userId,
            PersonhoodVerified = false,
            TrustLevel = "unknown",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        dbContext.UserTrusts.Add(row);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return row;
    }
}


