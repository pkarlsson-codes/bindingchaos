using BindingChaos.IdentityProfile.Domain.Entities;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;
using BindingChaos.IdentityProfile.Infrastructure.Utilities;

namespace BindingChaos.IdentityProfile.Application.Commands;

/// <summary>
/// Command to create a society invite link on behalf of a society member.
/// </summary>
/// <param name="ActorId">The participant ID of the member creating the link.</param>
/// <param name="SocietyId">The ID of the society the invite link is for.</param>
/// <param name="Note">An optional private note visible only to the creator.</param>
public sealed record CreateSocietyInviteLink(string ActorId, string SocietyId, string? Note);

/// <summary>
/// Handles the <see cref="CreateSocietyInviteLink"/> command.
/// </summary>
public static class CreateSocietyInviteLinkHandler
{
    /// <summary>
    /// Creates a society invite link and returns its ID.
    /// </summary>
    /// <param name="command">The command containing creator and society identifiers.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created invite link.</returns>
    public static async Task<Guid> Handle(
        CreateSocietyInviteLink command,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var inviteLink = new SocietyInviteLink
        {
            Id = Guid.NewGuid(),
            Token = InviteLinkTokenGenerator.Generate(),
            CreatedById = command.ActorId,
            SocietyId = command.SocietyId,
            Note = command.Note,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.SocietyInviteLinks.Add(inviteLink);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return inviteLink.Id;
    }
}
