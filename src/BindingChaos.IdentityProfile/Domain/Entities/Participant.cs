namespace BindingChaos.IdentityProfile.Domain.Entities;

/// <summary>
/// Stores the stable pseudonym assigned to a participant at registration.
/// </summary>
public sealed class Participant
{
    /// <summary>Gets the internal stable user identifier.</summary>
    public required string UserId { get; init; }

    /// <summary>Gets the globally unique pseudonym for this participant.</summary>
    public required string Pseudonym { get; init; }

    /// <summary>Gets the UTC timestamp when this participant record was created.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
}
