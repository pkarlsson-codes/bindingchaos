namespace BindingChaos.Ideation.Application.ReadModels;

/// <summary>
/// View used to project amendment opponent information.
/// </summary>
public class AmendmentOpponentView
{
    /// <summary>
    /// The unique identifier for this opponent record.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The amendment identifier.
    /// </summary>
    public string AmendmentId { get; set; } = string.Empty;

    /// <summary>
    /// The participant identifier who opposes the amendment.
    /// </summary>
    public string ParticipantId { get; set; } = string.Empty;

    /// <summary>
    /// The reason provided by the opponent for opposing the amendment.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// When the opposition was added.
    /// </summary>
    public DateTimeOffset OpposedAt { get; set; }
}
