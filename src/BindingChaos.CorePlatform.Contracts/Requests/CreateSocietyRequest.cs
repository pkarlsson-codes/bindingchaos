namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for creating a new society.
/// </summary>
/// <param name="Name">The name of the society.</param>
/// <param name="Description">The description of the society.</param>
/// <param name="Tags">The initial tags for the society.</param>
/// <param name="RatificationThreshold">The decision protocol ratification threshold (e.g., 0.5 for 50%).</param>
/// <param name="ReviewWindowHours">The decision protocol review window in hours.</param>
/// <param name="AllowVeto">Whether a single principled objection can block a decision.</param>
/// <param name="RequiredVerificationWeight">The minimum verification weight required for signals.</param>
/// <param name="GeographicBounds">Optional geographic bounds for the society.</param>
/// <param name="Center">Optional center coordinates for the society.</param>
public record CreateSocietyRequest(
    string Name,
    string Description,
    string[] Tags,
    double RatificationThreshold,
    double ReviewWindowHours,
    bool AllowVeto,
    double RequiredVerificationWeight,
    GeographicBoundsRequest? GeographicBounds,
    CoordinatesRequest? Center);

/// <summary>
/// Represents geographic bounding box coordinates in a request.
/// </summary>
/// <param name="North">The northern boundary in degrees.</param>
/// <param name="South">The southern boundary in degrees.</param>
/// <param name="East">The eastern boundary in degrees.</param>
/// <param name="West">The western boundary in degrees.</param>
public record GeographicBoundsRequest(double North, double South, double East, double West);

/// <summary>
/// Represents geographic coordinates in a request.
/// </summary>
/// <param name="Latitude">The latitude in degrees (-90 to 90).</param>
/// <param name="Longitude">The longitude in degrees (-180 to 180).</param>
public record CoordinatesRequest(double Latitude, double Longitude);
