namespace BindingChaos.SharedKernel.Domain.Geography;

/// <summary>
/// Geographic area value object representing a bounded region.
/// </summary>
public sealed class GeographicArea : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the GeographicArea class.
    /// </summary>
    /// <param name="north">The northern boundary.</param>
    /// <param name="south">The southern boundary.</param>
    /// <param name="east">The eastern boundary.</param>
    /// <param name="west">The western boundary.</param>
    public GeographicArea(double north, double south, double east, double west)
    {
        if (north <= south)
        {
            throw new ArgumentException("North must be greater than South");
        }

        if (east <= west)
        {
            throw new ArgumentException("East must be greater than West");
        }

        North = north;
        South = south;
        East = east;
        West = west;
    }

    /// <summary>
    /// Gets the northern boundary in degrees.
    /// </summary>
    public double North { get; }

    /// <summary>
    /// Gets the southern boundary in degrees.
    /// </summary>
    public double South { get; }

    /// <summary>
    /// Gets the eastern boundary in degrees.
    /// </summary>
    public double East { get; }

    /// <summary>
    /// Gets the western boundary in degrees.
    /// </summary>
    public double West { get; }

    /// <summary>
    /// Gets the center coordinates of this geographic area.
    /// </summary>
    public Coordinates Center => new Coordinates((North + South) / 2, (East + West) / 2);

    /// <summary>
    /// Determines if the specified coordinates are within this geographic area.
    /// </summary>
    /// <param name="coords">The coordinates to check.</param>
    /// <returns>True if the coordinates are within this area, false otherwise.</returns>
    public bool Contains(Coordinates coords)
    {
        return coords.Latitude >= South && coords.Latitude <= North &&
               coords.Longitude >= West && coords.Longitude <= East;
    }

    /// <summary>
    /// Gets the equality components for this value object.
    /// </summary>
    /// <returns>The equality components.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return North;
        yield return South;
        yield return East;
        yield return West;
    }
}
