namespace BindingChaos.Societies.Application.ReadModels;

/// <summary>
/// Read model for a single active member of a society, keyed by membership ID.
/// </summary>
public class SocietyMemberView
{
    /// <summary>
    /// Gets or sets the membership identifier (used as the document key).
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the society identifier.
    /// </summary>
    public string SocietyId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the participant identifier.
    /// </summary>
    public string ParticipantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the social contract identifier agreed to at join time.
    /// </summary>
    public string SocialContractId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the participant joined.
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }

    /// <summary>
    /// Gets or sets whether this membership is currently active.
    /// Set to <see langword="false"/> when the participant leaves.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the invite token used to join, if any. Stored for attribution.
    /// </summary>
    public string? InviteToken { get; set; }
}
