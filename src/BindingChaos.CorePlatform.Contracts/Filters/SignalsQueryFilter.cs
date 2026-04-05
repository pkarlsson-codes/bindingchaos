namespace BindingChaos.CorePlatform.Contracts.Filters;

/// <summary>
/// Filter for querying signals.
/// </summary>
public record SignalsQueryFilter
{
    /// <summary>
    /// Optional time window to filter signals, for example "24h" or "7d".
    /// </summary>
    public string? TimeRange { get; set; }

    /// <summary>
    /// Optional amplification level filter (e.g., "low", "medium", "high").
    /// </summary>
    public string? AmplificationLevel { get; set; }

    /// <summary>
    /// Optional free-text search term to match against signal content.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Optional collection of tags to filter results by.
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// Optional participant ID to filter signals amplified by that participant.
    /// </summary>
    public string? AmplifiedByParticipantId { get; set; }

    /// <summary>
    /// Optional participant ID to filter signals captured by that participant.
    /// </summary>
    public string? CapturedByParticipantId { get; set; }
}
