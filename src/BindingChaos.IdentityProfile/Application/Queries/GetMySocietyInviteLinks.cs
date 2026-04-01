using BindingChaos.IdentityProfile.Application.ReadModels;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Application.Queries;

/// <summary>
/// Query to retrieve all society invite links created by a participant for a given society.
/// </summary>
/// <param name="ActorId">The participant ID of the requesting member.</param>
/// <param name="SocietyId">The ID of the society whose invite links are being retrieved.</param>
public sealed record GetMySocietyInviteLinks(string ActorId, string SocietyId);

/// <summary>
/// Handles the <see cref="GetMySocietyInviteLinks"/> query.
/// </summary>
public static class GetMySocietyInviteLinksHandler
{
    /// <summary>
    /// Returns all invite links created by the participant for the specified society, newest first.
    /// </summary>
    /// <param name="query">The query containing the participant and society identifiers.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All invite links (active and revoked) created by the participant for the society.</returns>
    public static async Task<IReadOnlyList<SocietyInviteLinkView>> Handle(
        GetMySocietyInviteLinks query,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await dbContext.SocietyInviteLinks
            .Where(l => l.CreatedById == query.ActorId && l.SocietyId == query.SocietyId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new SocietyInviteLinkView(l.Id, l.Token, l.SocietyId, l.Note, l.IsRevoked, l.CreatedAt))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
