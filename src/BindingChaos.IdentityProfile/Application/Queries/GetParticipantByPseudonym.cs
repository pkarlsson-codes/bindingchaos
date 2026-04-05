using BindingChaos.IdentityProfile.Application.ReadModels;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Application.Queries;

/// <summary>
/// Query to look up a participant by their pseudonym.
/// </summary>
/// <param name="Pseudonym">The pseudonym to resolve.</param>
public sealed record GetParticipantByPseudonym(string Pseudonym);

/// <summary>
/// Handles the <see cref="GetParticipantByPseudonym"/> query.
/// </summary>
public static class GetParticipantByPseudonymHandler
{
    /// <summary>
    /// Returns the participant matching the given pseudonym, or null if not found.
    /// </summary>
    /// <param name="query">The query containing the pseudonym to resolve.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="ParticipantView"/> if found; otherwise null.</returns>
    public static async Task<ParticipantView?> Handle(
        GetParticipantByPseudonym query,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await dbContext.Participants
            .Where(p => p.Pseudonym == query.Pseudonym)
            .Select(p => new ParticipantView(p.UserId, p.Pseudonym, p.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
