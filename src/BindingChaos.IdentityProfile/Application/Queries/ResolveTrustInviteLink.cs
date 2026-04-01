using BindingChaos.IdentityProfile.Application.ReadModels;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Application.Queries;

/// <summary>
/// Query to resolve an invite link token to the inviter's user ID.
/// </summary>
/// <param name="Token">The URL-safe base64url token from the invite link.</param>
public sealed record ResolveTrustInviteLink(string Token);

/// <summary>
/// Handles the <see cref="ResolveTrustInviteLink"/> query.
/// </summary>
public static class ResolveTrustInviteLinkHandler
{
    /// <summary>
    /// Returns the inviter's user ID for a valid, non-revoked token, or null if not found or revoked.
    /// </summary>
    /// <param name="query">The query containing the token to resolve.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ResolvedInviteLinkView"/> if the token is valid; otherwise null.</returns>
    public static async Task<ResolvedInviteLinkView?> Handle(
        ResolveTrustInviteLink query,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var link = await dbContext.TrustTrustInviteLinks
            .Where(l => l.Token == query.Token && !l.IsRevoked)
            .Select(l => new ResolvedInviteLinkView(l.CreatedById))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return link;
    }
}
