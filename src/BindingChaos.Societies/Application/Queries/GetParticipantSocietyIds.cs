using BindingChaos.SharedKernel.Domain;
using BindingChaos.Societies.Application.ReadModels;
using Marten;

namespace BindingChaos.Societies.Application.Queries;

/// <summary>
/// Query to retrieve the IDs of all societies a participant is an active member of.
/// </summary>
/// <param name="ParticipantId">The participant whose memberships to look up.</param>
public sealed record GetParticipantSocietyIds(ParticipantId ParticipantId);

/// <summary>
/// Handles the <see cref="GetParticipantSocietyIds"/> query.
/// </summary>
public static class GetParticipantSocietyIdsHandler
{
    /// <summary>
    /// Returns the society IDs of all societies the participant is currently an active member of.
    /// </summary>
    /// <param name="request">The query containing the participant ID.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of society ID strings.</returns>
    public static async Task<string[]> Handle(
        GetParticipantSocietyIds request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var societyIds = await querySession.Query<SocietyMemberView>()
            .Where(m => m.ParticipantId == request.ParticipantId.Value && m.IsActive)
            .Select(m => m.SocietyId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return [.. societyIds];
    }
}
