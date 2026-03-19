namespace BindingChaos.Ideation.Application.ReadModels;

/// <summary>
/// View used to project amendment supporter information.
/// </summary>
public class AmendmentSupporterView
{
    /// <summary>
    /// The unique identifier for this supporter record.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The amendment identifier.
    /// </summary>
    public string AmendmentId { get; set; } = string.Empty;

    /// <summary>
    /// The participant identifier who supports the amendment.
    /// </summary>
    public string ParticipantId { get; set; } = string.Empty;

    /// <summary>
    /// The reason provided by the supporter for supporting the amendment.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// When the support was added.
    /// </summary>
    public DateTimeOffset SupportedAt { get; set; }
}
