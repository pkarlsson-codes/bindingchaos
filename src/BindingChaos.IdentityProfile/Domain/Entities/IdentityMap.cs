namespace BindingChaos.IdentityProfile.Domain.Entities;

/// <summary>
/// Maps external identity (provider + subject) to an internal stable userId.
/// </summary>
public sealed class IdentityMap
{
    public required string Provider { get; set; } = default!;

    public required string Sub { get; set; } = default!;

    public required string UserId { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }
}


