namespace BindingChaos.IdentityProfile.Application.ReadModels;

/// <summary>
/// The result of a successful <see cref="Commands.CreateTrustInviteLink"/> command.
/// </summary>
/// <param name="Id">The newly created invite link's ID.</param>
/// <param name="Token">The URL-safe base64url token.</param>
/// <param name="Note">The optional note supplied by the creator.</param>
/// <param name="CreatedAt">UTC timestamp of creation.</param>
public sealed record TrustInviteLinkCreatedView(Guid Id, string Token, string? Note, DateTimeOffset CreatedAt);
