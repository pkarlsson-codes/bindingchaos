using BindingChaos.IdentityProfile.Domain.Entities;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using BindingChaos.SharedKernel.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.IdentityProfile.Application.Commands;

/// <summary>
/// Command to revoke an invite link.
/// </summary>
/// <param name="Id">The ID of the invite link to revoke.</param>
/// <param name="RequestorUserId">The internal user ID of the participant requesting revocation.</param>
public sealed record RevokeInviteLink(Guid Id, string RequestorUserId);

/// <summary>
/// Handles the <see cref="RevokeInviteLink"/> command.
/// </summary>
public static class RevokeInviteLinkHandler
{
    /// <summary>
    /// Loads the invite link, verifies ownership, and sets <c>IsRevoked = true</c>.
    /// Idempotent if the link is already revoked.
    /// </summary>
    /// <param name="command">The revocation command.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="AggregateNotFoundException">Thrown when no invite link with <paramref name="command"/>.Id exists.</exception>
    /// <exception cref="ForbiddenException">Thrown when the requestor is not the creator of the link.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        RevokeInviteLink command,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var link = await dbContext.InviteLinks
            .FirstOrDefaultAsync(l => l.Id == command.Id, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AggregateNotFoundException(typeof(InviteLink), command.Id);

        if (link.CreatorUserId != command.RequestorUserId)
        {
            throw new ForbiddenException($"Participant {command.RequestorUserId} does not own invite link {command.Id}.");
        }

        if (!link.IsRevoked)
        {
            link.IsRevoked = true;
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
