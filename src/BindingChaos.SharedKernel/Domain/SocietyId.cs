namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Unique identifier for a Society.
/// </summary>
public class SocietyId : EntityId<SocietyId>
{
    private const string Prefix = "society";

    /// <summary>
    /// Initializes a new instance of the <see cref="SocietyId"/> class.
    /// </summary>
    /// <param name="value">The unique identifier value.</param>
    public SocietyId(string value)
        : base(value, Prefix)
    {
    }
}
