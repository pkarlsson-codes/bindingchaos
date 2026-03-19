namespace BindingChaos.SharedKernel.Domain.Geography;

/// <summary>
/// Geographic coordinates value object.
/// </summary>
public sealed class Coordinates : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the Coordinates class.
    /// </summary>
    /// <param name="latitude">The latitude in degrees (-90 to 90).</param>
    /// <param name="longitude">The longitude in degrees (-180 to 180).</param>
    public Coordinates(double latitude, double longitude)
    {
        if (!double.IsFinite(latitude))
        {
            throw new ArgumentException("Latitude must be a finite number", nameof(latitude));
        }

        if (!double.IsFinite(longitude))
        {
            throw new ArgumentException("Longitude must be a finite number", nameof(longitude));
        }

        if (latitude is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees");
        }

        if (longitude is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees");
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Gets the latitude coordinate in degrees.
    /// </summary>
    public double Latitude { get; }

    /// <summary>
    /// Gets the longitude coordinate in degrees.
    /// </summary>
    public double Longitude { get; }

    /// <summary>
    /// Gets the equality components for this value object.
    /// </summary>
    /// <returns>The equality components.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}
