using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Application.Services;

/// <summary>
/// Queries the identity store to resolve stable pseudonyms for participant IDs.
/// </summary>
public sealed class PseudonymLookupService(IdentityProfileDbContext dbContext) : IPseudonymLookupService
{
    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<string, string>> GetPseudonymsAsync(
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.ToList();
        var results = await dbContext.Participants
            .AsNoTracking()
            .Where(p => ids.Contains(p.UserId))
            .Select(p => new { p.UserId, p.Pseudonym })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return results.ToDictionary(p => p.UserId, p => p.Pseudonym);
    }

    /// <inheritdoc />
    public async Task<string?> GetPseudonymAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Participants
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => p.Pseudonym)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
