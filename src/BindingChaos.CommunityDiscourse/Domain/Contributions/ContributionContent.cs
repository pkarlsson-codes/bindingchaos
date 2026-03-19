using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.CommunityDiscourse.Domain.Contributions;

/// <summary>
/// Value object representing contribution content with validation and formatting rules.
/// </summary>
public sealed class ContributionContent : ValueObject
{
    private const int MaxContentLength = 10000;
    private const int MinContentLength = 1;

    /// <summary>
    /// Initializes a new instance of the ContributionContent class.
    /// </summary>
    /// <param name="value">The content value.</param>
    private ContributionContent(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the content value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new ContributionContent from a string value.
    /// </summary>
    /// <param name="value">The content value.</param>
    /// <returns>A new ContributionContent instance.</returns>
    /// <exception cref="ArgumentException">Thrown when content is invalid.</exception>
    public static ContributionContent Create(string value)
    {
        ValidateContent(value);
        return new ContributionContent(value.Trim());
    }

    /// <summary>
    /// Tries to create a new ContributionContent from a string value.
    /// </summary>
    /// <param name="value">The content value.</param>
    /// <returns>A new ContributionContent instance, or null if invalid.</returns>
    public static ContributionContent? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            ValidateContent(value);
            return new ContributionContent(value.Trim());
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the components used for equality comparison.
    /// </summary>
    /// <returns>The components for equality comparison.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Validates the content value according to domain rules.
    /// </summary>
    /// <param name="value">The content to validate.</param>
    /// <exception cref="ArgumentException">Thrown when content is invalid.</exception>
    private static void ValidateContent(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Content cannot be null, empty, or whitespace.", nameof(value));
        }

        var trimmedValue = value.Trim();
        if (trimmedValue.Length < MinContentLength)
        {
            throw new ArgumentException($"Content must be at least {MinContentLength} character long.", nameof(value));
        }

        if (trimmedValue.Length > MaxContentLength)
        {
            throw new ArgumentException($"Content cannot exceed {MaxContentLength} characters.", nameof(value));
        }
    }
}