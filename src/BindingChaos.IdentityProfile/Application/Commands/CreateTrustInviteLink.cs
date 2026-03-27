using BindingChaos.IdentityProfile.Application.ReadModels;
using BindingChaos.IdentityProfile.Domain.Entities;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;

namespace BindingChaos.IdentityProfile.Application.Commands;

/// <summary>
/// Command to create an invite link for a participant.
/// </summary>
/// <param name="CreatorUserId">The internal user ID of the participant creating the link.</param>
/// <param name="Note">An optional private note visible only to the creator.</param>
public sealed record CreateTrustInviteLink(string CreatorUserId, string? Note);

/// <summary>
/// Handles the <see cref="CreateTrustInviteLink"/> command.
/// </summary>
public static class CreateTrustInviteLinkHandler
{
    /// <summary>
    /// Handles the <see cref="CreateTrustInviteLink"/> command by generating a token, persisting the invite link, and returning its details.
    /// </summary>
    /// <param name="command">The command containing creator and optional note.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created invite link details.</returns>
    public static async Task<TrustInviteLinkCreatedView> Handle(
        CreateTrustInviteLink command,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tokenBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
        var token = Base64UrlEncode(tokenBytes);

        var inviteLink = new TrustInviteLink
        {
            Id = Guid.NewGuid(),
            Token = token,
            CreatorUserId = command.CreatorUserId,
            Note = command.Note,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.TrustTrustInviteLinks.Add(inviteLink);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new TrustInviteLinkCreatedView(inviteLink.Id, inviteLink.Token, inviteLink.Note, inviteLink.CreatedAt);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
