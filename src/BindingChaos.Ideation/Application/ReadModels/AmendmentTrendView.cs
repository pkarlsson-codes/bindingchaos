namespace BindingChaos.Ideation.Application.ReadModels;

/// <summary>
/// View used to project amendment support trend information over time.
/// </summary>
public class AmendmentTrendView
{
    /// <summary>
    /// The unique identifier for this trend view (same as AmendmentId).
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The amendment identifier.
    /// </summary>
    public string AmendmentId { get; set; } = string.Empty;

    /// <summary>
    /// The collection of trend data points showing supporter/opponent counts over time.
    /// </summary>
    public List<AmendmentTrendPoint> DataPoints { get; set; } = [];
}
