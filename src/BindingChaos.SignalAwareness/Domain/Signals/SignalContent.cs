using System.Runtime.CompilerServices;
using BindingChaos.SharedKernel.Domain;

[assembly: InternalsVisibleTo("BindingChaos.SignalAwareness.Tests")]

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Value object representing the content of a signal.
/// This value object is internal to the SignalAwareness bounded context.
/// </summary>
internal sealed class SignalContent : ValueObject
{
    private SignalContent(string title, string description)
    {
        Title = title;
        Description = description;
    }

    /// <summary>
    /// Gets the title of the signal.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the description of the signal.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Creates a new SignalContent instance with the specified title and description.
    /// </summary>
    /// <param name="title">The title of the signal. Must not be null or empty and cannot exceed 200 characters.</param>
    /// <param name="description">The description of the signal. Must not be null or empty and cannot exceed 2000 characters.</param>
    /// <returns>A new SignalContent instance.</returns>
    /// <exception cref="ArgumentException">Thrown when title or description validation fails.</exception>
    public static SignalContent Create(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        }

        if (title.Length > 200)
        {
            throw new ArgumentException("Title cannot exceed 200 characters", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or empty", nameof(description));
        }

        if (description.Length > 2000)
        {
            throw new ArgumentException("Description cannot exceed 2000 characters", nameof(description));
        }

        return new SignalContent(title.Trim(), description.Trim());
    }

    /// <summary>
    /// Returns a string representation of the signal content.
    /// </summary>
    /// <returns>A string in the format "Title: Description".</returns>
    public override string ToString()
    {
        return $"{Title}: {Description}";
    }

    /// <summary>
    /// Gets the components that make up the value of this object for equality comparison.
    /// </summary>
    /// <returns>An enumerable of objects that represent the value components.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Title;
        yield return Description;
    }
}