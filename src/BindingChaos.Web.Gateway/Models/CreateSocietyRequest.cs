namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Request model for creating a new society.
/// </summary>
public sealed class CreateSocietyRequest
{
    /// <summary>
    /// The name of the society.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the society.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The initial tags for the society.
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// The decision protocol ratification threshold (e.g., 0.5 for 50%).
    /// </summary>
    public double RatificationThreshold { get; set; }

    /// <summary>
    /// The decision protocol review window in hours.
    /// </summary>
    public double ReviewWindowHours { get; set; }

    /// <summary>
    /// Whether a single principled objection can block a decision.
    /// </summary>
    public bool AllowVeto { get; set; }

    /// <summary>
    /// The minimum verification weight required for signals.
    /// </summary>
    public double RequiredVerificationWeight { get; set; }

    /// <summary>
    /// How long an unanswered inquiry remains open before auto-lapsing, in hours.
    /// </summary>
    public double InquiryLapseWindowHours { get; set; }

    /// <summary>
    /// Optional geographic bounds for the society.
    /// </summary>
    public GeographicBoundsModel? GeographicBounds { get; set; }

    /// <summary>
    /// Optional center coordinates for the society.
    /// </summary>
    public CoordinatesModel? Center { get; set; }
}

/// <summary>
/// Represents geographic bounding box coordinates.
/// </summary>
public sealed class GeographicBoundsModel
{
    /// <summary>The northern boundary in degrees.</summary>
    public double North { get; set; }

    /// <summary>The southern boundary in degrees.</summary>
    public double South { get; set; }

    /// <summary>The eastern boundary in degrees.</summary>
    public double East { get; set; }

    /// <summary>The western boundary in degrees.</summary>
    public double West { get; set; }
}

/// <summary>
/// Represents geographic coordinates.
/// </summary>
public sealed class CoordinatesModel
{
    /// <summary>The latitude in degrees (-90 to 90).</summary>
    public double Latitude { get; set; }

    /// <summary>The longitude in degrees (-180 to 180).</summary>
    public double Longitude { get; set; }
}
