namespace BindingChaos.IdentityProfile.Domain.Entities;

/// <summary>
/// An invitation link that a participant can share to allow others to register.
/// </summary>
public sealed class InviteLink
{
    /// <summary>Gets the primary key.</summary>
    public required Guid Id { get; init; }

    /// <summary>Gets the URL-safe random token (22 chars, 16 bytes base64url) used in the invite URL.</summary>
    public required string Token { get; init; }

    /// <summary>Gets the user ID of the participant who created this invite link.</summary>
    public required string CreatorUserId { get; init; }

    /// <summary>Gets the optional private note visible only to the creator.</summary>
    public string? Note { get; init; }

    /// <summary>Gets a value indicating whether this invite link has been revoked.</summary>
    public bool IsRevoked { get; set; }

    /// <summary>Gets the UTC timestamp when this invite link was created.</summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets the creator participant navigation property.</summary>
    public Participant? Creator { get; init; }
}
