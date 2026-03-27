using BindingChaos.IdentityProfile.Application.ReadModels;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Application.Queries;

/// <summary>
/// Query to retrieve all invite links created by a participant.
/// </summary>
/// <param name="UserId">The internal user ID of the participant.</param>
public sealed record GetMyInviteLinks(string UserId);

/// <summary>
/// Handles the <see cref="GetMyInviteLinks"/> query.
/// </summary>
public static class GetMyInviteLinksHandler
{
    /// <summary>
    /// Returns all invite links for the participant, sorted by creation date descending.
    /// </summary>
    /// <param name="query">The query containing the participant's user ID.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All invite links (active and revoked) for the participant, newest first.</returns>
    public static async Task<IReadOnlyList<InviteLinkView>> Handle(
        GetMyInviteLinks query,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await dbContext.InviteLinks
            .Where(l => l.CreatorUserId == query.UserId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new InviteLinkView(l.Id, l.Token, l.Note, l.IsRevoked, l.CreatedAt))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
