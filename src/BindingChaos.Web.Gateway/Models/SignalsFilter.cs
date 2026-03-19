namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Filter parameters for the signals feed, bound from the querystring via the "filter.*" prefix.
/// </summary>
public sealed class SignalsFilter
{
    /// <summary>
    /// Time range filter: 24h, 7d, 30d, all.
    /// </summary>
    public string TimeRange { get; init; } = "all";

    /// <summary>
    /// Amplification level filter: all, high, medium, low.
    /// </summary>
    public string AmplificationLevel { get; init; } = "all";

    /// <summary>
    /// Search term for title and description.
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Comma-separated list of tags to filter by.
    /// </summary>
    public string? Tags { get; init; }
}
