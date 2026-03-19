using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Provides a base class for creating strongly-typed enumerations with associated display names and integer values.
/// </summary>
/// <typeparam name="T">The type of the enumeration that inherits from this base class.</typeparam>
public abstract class Enumeration<T>(int value, string displayName)
    : Enumeration(value, displayName)
    where T : Enumeration<T>
{
    /// <summary>
    /// Converts the specified integer value to an instance of type T.
    /// </summary>
    /// <remarks>This method is typically used for converting integer values that are defined in an
    /// enumeration or similar construct to their corresponding type T instances.</remarks>
    /// <param name="value">The integer value to convert. This value must correspond to a valid representation of type T.</param>
    /// <returns>An instance of type T that represents the specified integer value.</returns>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static T FromValue(int value)
        => FromValue<T>(value);

    /// <summary>
    /// Converts the specified display name to an instance of type T.
    /// </summary>
    /// <param name="displayName">The display name to convert. This value must not be null or empty.</param>
    /// <returns>An instance of type T that corresponds to the provided display name. Returns the default value of T if no match
    /// is found.</returns>
    public static T FromDisplayName(string displayName)
        => FromDisplayName<T>(displayName);

    /// <summary>
    /// Attempts to convert the specified integer value to an instance of type T.
    /// </summary>
    /// <remarks>This method is useful for safely attempting to convert an integer to a specific type without
    /// throwing exceptions. It is recommended to check the return value before using the result.</remarks>
    /// <param name="value">The integer value to convert to type T.</param>
    /// <param name="result">When this method returns, contains the converted value of type T if the conversion succeeded; otherwise, it is
    /// null.</param>
    /// <returns>true if the conversion succeeded; otherwise, false.</returns>
    public static bool TryFromValue(int value, [MaybeNullWhen(false)] out T result)
        => TryFromValue<T>(value, out result);

    /// <summary>
    /// Converts the specified display name to an instance of type T.
    /// </summary>
    /// <param name="displayName">The display name to convert. This value must not be null or empty.</param>
    /// <param name="result">When this method returns, contains the converted value of type T if the conversion succeeded; otherwise, it is
    /// null.</param>
    /// <returns>true if the conversion succeeded; otherwise, false.</returns>
    public static bool TryFromDisplayName(string displayName, [MaybeNullWhen(false)] out T result)
        => TryFromDisplayName<T>(displayName, out result);

    /// <summary>
    /// Retrieves all instances of the specified type from the underlying data source.
    /// </summary>
    /// <returns>An enumerable collection containing all instances of type T. The collection is empty if no instances are found.</returns>
    public static IEnumerable<T> GetAll()
        => GetAll<T>();
#pragma warning restore CA1000 // Do not declare static members on generic types
}

/// <summary>
/// Base class for rich enumeration patterns.
/// </summary>
public abstract class Enumeration : IComparable<Enumeration>, IEquatable<Enumeration>
{
    private readonly int _value;
    private readonly string _displayName;

    /// <summary>
    /// Initializes a new instance of the Enumeration class.
    /// </summary>
    /// <param name="value">The integer value of the enumeration.</param>
    /// <param name="displayName">The display name of the enumeration.</param>
    protected Enumeration(int value, string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentNullException(nameof(displayName), "Display name cannot be null or empty");
        }

        _value = value;
        _displayName = displayName;
    }

    /// <summary>
    /// Gets the integer value of the enumeration.
    /// </summary>
    public int Value => _value;

    /// <summary>
    /// Gets the display name of the enumeration.
    /// </summary>
    public string DisplayName => _displayName;

    /// <summary>
    /// Determines whether two enumerations are equal.
    /// </summary>
    /// <param name="left">The first enumeration to compare.</param>
    /// <param name="right">The second enumeration to compare.</param>
    /// <returns>True if the enumerations are equal; otherwise, false.</returns>
    public static bool operator ==(Enumeration? left, Enumeration? right)
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
    /// Determines whether two enumerations are not equal.
    /// </summary>
    /// <param name="left">The first enumeration to compare.</param>
    /// <param name="right">The second enumeration to compare.</param>
    /// <returns>True if the enumerations are not equal; otherwise, false.</returns>
    public static bool operator !=(Enumeration? left, Enumeration? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether the first enumeration is less than the second enumeration.
    /// </summary>
    /// <param name="left">The first enumeration to compare.</param>
    /// <param name="right">The second enumeration to compare.</param>
    /// <returns>True if the first enumeration is less than the second; otherwise, false.</returns>
    public static bool operator <(Enumeration? left, Enumeration? right)
    {
        return left?.CompareTo(right) < 0;
    }

    /// <summary>
    /// Determines whether the first enumeration is less than or equal to the second enumeration.
    /// </summary>
    /// <param name="left">The first enumeration to compare.</param>
    /// <param name="right">The second enumeration to compare.</param>
    /// <returns>True if the first enumeration is less than or equal to the second; otherwise, false.</returns>
    public static bool operator <=(Enumeration? left, Enumeration? right)
    {
        return left?.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Determines whether the first enumeration is greater than the second enumeration.
    /// </summary>
    /// <param name="left">The first enumeration to compare.</param>
    /// <param name="right">The second enumeration to compare.</param>
    /// <returns>True if the first enumeration is greater than the second; otherwise, false.</returns>
    public static bool operator >(Enumeration? left, Enumeration? right)
    {
        return left?.CompareTo(right) > 0;
    }

    /// <summary>
    /// Determines whether the first enumeration is greater than or equal to the second enumeration.
    /// </summary>
    /// <param name="left">The first enumeration to compare.</param>
    /// <param name="right">The second enumeration to compare.</param>
    /// <returns>True if the first enumeration is greater than or equal to the second; otherwise, false.</returns>
    public static bool operator >=(Enumeration? left, Enumeration? right)
    {
        return left?.CompareTo(right) >= 0;
    }

    /// <summary>
    /// Calculates the absolute difference between two enumeration values.
    /// </summary>
    /// <param name="firstValue">The first enumeration value.</param>
    /// <param name="secondValue">The second enumeration value.</param>
    /// <returns>The absolute difference between the values.</returns>
    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        if (firstValue.GetType() != secondValue.GetType())
        {
            throw new ArgumentException("Cannot calculate difference between enumerations of different types");
        }

        return Math.Abs(firstValue.Value - secondValue.Value);
    }

    /// <summary>
    /// Returns a string representation of the enumeration.
    /// </summary>
    /// <returns>The display name of the enumeration.</returns>
    public override string ToString() => DisplayName;

    /// <summary>
    /// Determines whether the specified object is equal to the current enumeration.
    /// </summary>
    /// <param name="obj">The object to compare with the current enumeration.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Enumeration);
    }

    /// <summary>
    /// Determines whether the specified enumeration is equal to the current enumeration.
    /// </summary>
    /// <param name="other">The enumeration to compare with the current enumeration.</param>
    /// <returns>True if the enumerations are equal; otherwise, false.</returns>
    public bool Equals(Enumeration? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return GetType() == other.GetType() && _value == other._value;
    }

    /// <summary>
    /// Gets the hash code for this enumeration.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), _value);
    }

    /// <summary>
    /// Compares the current enumeration with another enumeration.
    /// </summary>
    /// <param name="other">The enumeration to compare with.</param>
    /// <returns>A value indicating the relative order of the enumerations.</returns>
    public int CompareTo(Enumeration? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (GetType() != other.GetType())
        {
            throw new ArgumentException("Cannot compare enumerations of different types", nameof(other));
        }

        return _value.CompareTo(other._value);
    }

    /// <summary>
    /// Gets an enumeration instance by its value.
    /// </summary>
    /// <typeparam name="T">The type of enumeration.</typeparam>
    /// <param name="value">The value to search for.</param>
    /// <returns>The enumeration instance.</returns>
    protected static T FromValue<T>(int value)
        where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Value == value);
        return matchingItem;
    }

    /// <summary>
    /// Gets an enumeration instance by its display name.
    /// </summary>
    /// <typeparam name="T">The type of enumeration.</typeparam>
    /// <param name="displayName">The display name to search for.</param>
    /// <returns>The enumeration instance.</returns>
    protected static T FromDisplayName<T>(string displayName)
        where T : Enumeration
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.DisplayName == displayName);
        return matchingItem;
    }

    /// <summary>
    /// Gets all enumeration instances of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of enumeration.</typeparam>
    /// <returns>All enumeration instances.</returns>
    protected static IEnumerable<T> GetAll<T>()
        where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    /// <summary>
    /// Attempts to get an enumeration instance by its value.
    /// </summary>
    /// <typeparam name="T">The type of enumeration.</typeparam>
    /// <param name="value">The value to search for.</param>
    /// <param name="result">The enumeration instance, if found.</param>
    /// <returns>True if the enumeration was found; otherwise, false.</returns>
    protected static bool TryFromValue<T>(int value, [MaybeNullWhen(false)] out T result)
        where T : Enumeration
    {
        try
        {
            result = FromValue<T>(value);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to get an enumeration instance by its display name.
    /// </summary>
    /// <typeparam name="T">The type of enumeration.</typeparam>
    /// <param name="displayName">The display name to search for.</param>
    /// <param name="result">The enumeration instance, if found.</param>
    /// <returns>True if the enumeration was found; otherwise, false.</returns>
    protected static bool TryFromDisplayName<T>(string displayName, [MaybeNullWhen(false)] out T result)
        where T : Enumeration
    {
        try
        {
            result = FromDisplayName<T>(displayName);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    private static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate)
        where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
        {
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
        }

        return matchingItem;
    }
}