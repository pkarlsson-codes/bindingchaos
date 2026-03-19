using System.Text;

namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Helper utility for encoding and decoding cursors used in cursor-based pagination.
/// Cursors are opaque strings that encode position information for stable pagination.
/// </summary>
public static class CursorHelper
{
    private const char Delimiter = '|';

    /// <summary>
    /// Encodes a cursor from a timestamp and ID combination.
    /// </summary>
    /// <param name="timestamp">The timestamp component of the cursor.</param>
    /// <param name="id">The ID component of the cursor.</param>
    /// <returns>A base64-encoded cursor string.</returns>
    public static string EncodeCursor(DateTimeOffset timestamp, string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var cursorData = $"{timestamp:O}{Delimiter}{id}";
        var bytes = Encoding.UTF8.GetBytes(cursorData);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Decodes a cursor to extract the timestamp and ID components.
    /// </summary>
    /// <param name="cursor">The base64-encoded cursor string.</param>
    /// <returns>A tuple containing the decoded timestamp and ID.</returns>
    /// <exception cref="ArgumentException">Thrown when the cursor format is invalid.</exception>
    public static (DateTimeOffset Timestamp, string Id) DecodeCursor(string cursor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cursor);

        try
        {
            var bytes = Convert.FromBase64String(cursor);
            var cursorData = Encoding.UTF8.GetString(bytes);

            var delimiterIndex = cursorData.IndexOf(Delimiter);
            if (delimiterIndex == -1)
            {
                throw new ArgumentException("Invalid cursor format: missing delimiter", nameof(cursor));
            }

            var timestampSpan = cursorData.AsSpan(0, delimiterIndex);
            var idSpan = cursorData.AsSpan(delimiterIndex + 1);

            if (idSpan.IsWhiteSpace())
            {
                throw new ArgumentException("Invalid cursor format: empty ID", nameof(cursor));
            }

            if (!DateTimeOffset.TryParse(timestampSpan, out var timestamp))
            {
                throw new ArgumentException("Invalid cursor format: invalid timestamp", nameof(cursor));
            }

            var id = idSpan.ToString();

            return (timestamp, id);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid cursor format: not valid base64", nameof(cursor), ex);
        }
    }

    /// <summary>
    /// Creates a next cursor from a collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="items">The collection of items.</param>
    /// <param name="timestampSelector">Function to extract timestamp from an item.</param>
    /// <param name="idSelector">Function to extract ID from an item.</param>
    /// <returns>The encoded cursor for the last item, or null if no items.</returns>
    public static string? CreateNextCursor<T>(
        IEnumerable<T> items,
        Func<T, DateTimeOffset> timestampSelector,
        Func<T, string> idSelector)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(timestampSelector);
        ArgumentNullException.ThrowIfNull(idSelector);

        var lastItem = items.LastOrDefault();
        if (lastItem == null)
        {
            return null;
        }

        return EncodeCursor(timestampSelector(lastItem), idSelector(lastItem));
    }

    /// <summary>
    /// Creates a previous cursor from a collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="items">The collection of items.</param>
    /// <param name="timestampSelector">Function to extract timestamp from an item.</param>
    /// <param name="idSelector">Function to extract ID from an item.</param>
    /// <returns>The encoded cursor for the first item, or null if no items.</returns>
    public static string? CreatePreviousCursor<T>(
        IEnumerable<T> items,
        Func<T, DateTimeOffset> timestampSelector,
        Func<T, string> idSelector)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(timestampSelector);
        ArgumentNullException.ThrowIfNull(idSelector);

        var firstItem = items.FirstOrDefault();
        if (firstItem == null)
        {
            return null;
        }

        return EncodeCursor(timestampSelector(firstItem), idSelector(firstItem));
    }

    /// <summary>
    /// Validates that a cursor string is properly formatted.
    /// </summary>
    /// <param name="cursor">The cursor to validate.</param>
    /// <returns>True if the cursor is valid, false otherwise.</returns>
    public static bool IsValidCursor(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return false;
        }

        try
        {
            DecodeCursor(cursor);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}