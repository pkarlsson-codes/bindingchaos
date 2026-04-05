using BindingChaos.IdentityProfile.Application.ReadModels;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Application.Queries;

/// <summary>
/// Query to look up a participant by their internal user ID.
/// </summary>
/// <param name="UserId">The user ID to resolve.</param>
public sealed record GetParticipantByUserId(string UserId);

/// <summary>
/// Handles the <see cref="GetParticipantByUserId"/> query.
/// </summary>
public static class GetParticipantByUserIdHandler
{
    /// <summary>
    /// Returns the participant matching the given user ID, or null if not found.
    /// </summary>
    /// <param name="query">The query containing the user ID to resolve.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ParticipantView"/> if found; otherwise null.</returns>
    public static async Task<ParticipantView?> Handle(
        GetParticipantByUserId query,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await dbContext.Participants
            .Where(p => p.UserId == query.UserId)
            .Select(p => new ParticipantView(p.UserId, p.Pseudonym, p.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
