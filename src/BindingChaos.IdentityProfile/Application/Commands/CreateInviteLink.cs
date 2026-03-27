using BindingChaos.IdentityProfile.Domain.Entities;
using BindingChaos.IdentityProfile.Infrastructure.Persistence;

namespace BindingChaos.IdentityProfile.Application.Commands;

/// <summary>
/// Command to create an invite link for a participant.
/// </summary>
/// <param name="CreatorUserId">The internal user ID of the participant creating the link.</param>
/// <param name="Note">An optional private note visible only to the creator.</param>
public sealed record CreateInviteLink(string CreatorUserId, string? Note);

/// <summary>
/// The result of a successful <see cref="CreateInviteLink"/> command.
/// </summary>
/// <param name="Id">The newly created invite link's ID.</param>
/// <param name="Token">The URL-safe base64url token.</param>
/// <param name="Note">The optional note supplied by the creator.</param>
/// <param name="CreatedAt">UTC timestamp of creation.</param>
public sealed record InviteLinkCreatedResponse(Guid Id, string Token, string? Note, DateTimeOffset CreatedAt);

/// <summary>
/// Handles the <see cref="CreateInviteLink"/> command.
/// </summary>
public static class CreateInviteLinkHandler
{
    /// <summary>
    /// Handles the <see cref="CreateInviteLink"/> command by generating a token, persisting the invite link, and returning its details.
    /// </summary>
    /// <param name="command">The command containing creator and optional note.</param>
    /// <param name="dbContext">The identity profile database context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created invite link details.</returns>
    public static async Task<InviteLinkCreatedResponse> Handle(
        CreateInviteLink command,
        IdentityProfileDbContext dbContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tokenBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
        var token = Base64UrlEncode(tokenBytes);

        var inviteLink = new InviteLink
        {
            Id = Guid.NewGuid(),
            Token = token,
            CreatorUserId = command.CreatorUserId,
            Note = command.Note,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.InviteLinks.Add(inviteLink);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new InviteLinkCreatedResponse(inviteLink.Id, inviteLink.Token, inviteLink.Note, inviteLink.CreatedAt);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
