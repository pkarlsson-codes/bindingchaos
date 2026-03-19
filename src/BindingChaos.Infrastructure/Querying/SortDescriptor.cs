namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Describes a single sort operation against a logical field.
/// </summary>
public sealed class SortDescriptor
{
    /// <summary>
    /// Creates a new <see cref="SortDescriptor"/>.
    /// </summary>
    /// <param name="field">Logical field name to sort by.</param>
    /// <param name="direction">Sort direction.</param>
    public SortDescriptor(string field, SortDirection direction = SortDirection.Asc)
    {
        Field = field;
        Direction = direction;
    }

    /// <summary>
    /// Logical field name to sort by. Mapping to actual data model is responsibility of the consumer.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Sort direction.
    /// </summary>
    public SortDirection Direction { get; }
}
