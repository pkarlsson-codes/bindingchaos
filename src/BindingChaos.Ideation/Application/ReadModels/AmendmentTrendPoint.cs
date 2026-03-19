namespace BindingChaos.Ideation.Application.ReadModels;

/// <summary>
/// Represents a single vote event in the amendment support trend.
/// </summary>
public class AmendmentTrendPoint
{
    /// <summary>
    /// The date and time of this vote event.
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// The type of vote event: "support", "oppose", "withdraw_support", or "withdraw_oppose".
    /// </summary>
    public string VoteType { get; set; } = string.Empty;
}
