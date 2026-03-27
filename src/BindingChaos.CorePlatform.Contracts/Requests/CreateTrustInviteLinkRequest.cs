namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request body for creating an invite link.
/// </summary>
public sealed class CreateTrustInviteLinkRequest
{
    /// <summary>
    /// An optional private note visible only to the creator.
    /// </summary>
    public string? Note { get; init; }
}
