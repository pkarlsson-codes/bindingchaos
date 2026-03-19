using System.Reflection;

namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Prototype: Strongly-typed entity ID with prefix validation via constructor argument and static Create/Generate methods.
/// </summary>
public abstract class EntityId : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityId"/> class.
    /// </summary>
    /// <param name="value">The string value for this entity identifier.</param>
    /// <param name="prefix">The required prefix for this entity identifier.</param>
    /// <exception cref="ArgumentException">Thrown if value or prefix is null/empty, or value does not start with prefix.</exception>
    protected EntityId(string value, string? prefix)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("ID value cannot be null or empty.", nameof(value));
        }

        if (prefix is not null)
        {
            if (!value.StartsWith(prefix + "-", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"ID value must start with prefix '{prefix}-'.", nameof(value));
            }
        }

        Value = value;
    }

    /// <summary>
    /// Gets the string value of this entity identifier.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

/// <summary>
/// Generic base class for strongly-typed entity IDs with static Create/Generate methods.
/// </summary>
/// <typeparam name="TId">The concrete ID type.</typeparam>
public abstract class EntityId<TId> : EntityId
    where TId : EntityId<TId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityId{TId}"/> class.
    /// </summary>
    /// <param name="value">The string value for this entity identifier.</param>
    /// <param name="requiredPrefix">The required prefix for this entity identifier.</param>
    protected EntityId(string value, string requiredPrefix)
        : base(value, requiredPrefix)
    {
    }

    /// <summary>
    /// Creates a new ID instance from a value.
    /// </summary>
    /// <param name="value">The string value for the ID.</param>
    /// <returns>A new instance of <typeparamref name="TId"/>.</returns>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static TId Create(string value)
    {
        var ctor = typeof(TId).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(string)], null)
            ?? throw new InvalidOperationException($"Type {typeof(TId)} must have a constructor with a single string argument.");
        try
        {
            return (TId)ctor.Invoke([value]);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw; // unreachable
        }
    }

    /// <summary>
    /// Generates a new ID instance with a new ULID value and the required prefix.
    /// </summary>
    /// <returns>A new instance of <typeparamref name="TId"/>.</returns>
    public static TId Generate()
    {
        var prefix = GetPrefix();
        var ulid = NUlid.Ulid.NewUlid().ToString().ToLowerInvariant();
        var value = $"{prefix}-{ulid}";
        return Create(value);
    }
#pragma warning restore CA1000 // Do not declare static members on generic types

    /// <summary>
    /// Returns a string representing the id.
    /// </summary>
    /// <returns>Returns the id value.</returns>
    public override string ToString()
    {
        return Value;
    }

    private static string GetPrefix()
    {
        var field = typeof(TId).GetField("Prefix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"Type {typeof(TId)} must declare a const string Prefix field.");
        }

        return (string)field.GetRawConstantValue()!;
    }
}