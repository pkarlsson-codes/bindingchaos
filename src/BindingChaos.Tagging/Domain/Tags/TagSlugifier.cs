namespace BindingChaos.Tagging.Domain.Tags;

/// <summary>
/// Provides consistent slug normalization for tags across the domain.
/// </summary>
public static class TagSlugifier
{
    /// <summary>
    /// Converts the specified string to a URL-friendly slug by trimming whitespace and converting all characters to
    /// lowercase.
    /// </summary>
    /// <param name="input">The input string to be converted. Cannot be <see langword="null"/>.</param>
    /// <returns>A slugified version of the input string, with leading and trailing whitespace removed and all characters in
    /// lowercase.</returns>
    public static string Slugify(string input)
        => input.Trim().ToLowerInvariant(); // you can expand with ascii-folding, hyphens, etc.
}