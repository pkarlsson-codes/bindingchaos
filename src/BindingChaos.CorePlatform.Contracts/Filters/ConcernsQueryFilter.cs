namespace BindingChaos.CorePlatform.Contracts.Filters;

/// <summary>
/// Filter for querying concerns.
/// </summary>
public record ConcernsQueryFilter
{
    /// <summary>
    /// Optional participant ID to filter concerns raised by that participant.
    /// </summary>
    public string? RaisedByParticipantId { get; set; }
}
