namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Base class for all value objects in the domain.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>True if the value objects are not equal; otherwise, false.</returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>True if the value objects are equal; otherwise, false.</returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value object.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not ValueObject other || other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Gets the hash code for this value object.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        var hashCode = default(HashCode);
        foreach (var component in GetEqualityComponents())
        {
            hashCode.Add(component);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Returns a string representation of this value object.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString()
    {
        var components = GetEqualityComponents().ToArray();
        if (components.Length == 0)
        {
            return GetType().Name;
        }

        var componentStrings = components.Select(c => c?.ToString() ?? "null");
        return $"{GetType().Name}({string.Join(", ", componentStrings)})";
    }

    /// <summary>
    /// Gets the components that make up the value of this object for equality comparison.
    /// </summary>
    /// <returns>An enumerable of objects that represent the value components.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();
}
