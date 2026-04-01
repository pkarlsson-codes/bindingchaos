namespace BindingChaos.IdentityProfile.Domain.Entities;

/// <summary>
/// An invitation link that a society member can share to allow others to join the society.
/// </summary>
public sealed class SocietyInviteLink
{
    /// <summary>Gets the primary key.</summary>
    public required Guid Id { get; init; }

    /// <summary>Gets the URL-safe random token (22 chars, 16 bytes base64url) used in the invite URL.</summary>
    public required string Token { get; init; }

    /// <summary>Gets the participant ID of the member who created this invite link.</summary>
    public required string CreatedById { get; init; }

    /// <summary>Gets the ID of the society this invite link is for.</summary>
    public required string SocietyId { get; init; }

    /// <summary>An optional private note visible only to the creator.</summary>
    public required string? Note { get; set; }

    /// <summary>Gets a value indicating whether this invite link has been revoked.</summary>
    public bool IsRevoked { get; set; }

    /// <summary>Gets the UTC timestamp when this invite link was created.</summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets the creator participant navigation property.</summary>
    public Participant? Creator { get; init; }
}
