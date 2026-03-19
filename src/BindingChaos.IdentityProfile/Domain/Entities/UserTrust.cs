namespace BindingChaos.IdentityProfile.Domain.Entities;

/// <summary>
/// Stores trust metadata for a user.
/// </summary>
public sealed class UserTrust
{
    public required string UserId { get; set; } = default!;

    public bool PersonhoodVerified { get; set; }

    public string TrustLevel { get; set; } = "unknown";

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}


