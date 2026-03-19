using System.Security.Cryptography;
using System.Text;

namespace BindingChaos.Pseudonymity.Domain;

/// <summary>
/// Generates context-scoped pseudonyms using HMAC-based deterministic generation.
/// </summary>
internal static class PseudonymGenerator
{
    private const char Separator = '-';

    /// <summary>
    /// Generates a deterministic pseudonym using HMAC-SHA256 with the pattern: adjective-noun-word.
    /// The same combination of inputs will always produce the same pseudonym.
    /// </summary>
    /// <param name="secretKey">The secret key for HMAC.</param>
    /// <param name="aggregateIdType">The aggregate type name.</param>
    /// <param name="aggregateIdValue">The aggregate ID value.</param>
    /// <param name="userId">The user ID.</param>
    /// <returns>A deterministic pseudonym for this combination of inputs.</returns>
    /// <exception cref="ArgumentException">Thrown when any parameter is null or whitespace.</exception>
    public static string GeneratePseudonym(string secretKey, string aggregateIdType, string aggregateIdValue, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(aggregateIdType);
        ArgumentException.ThrowIfNullOrWhiteSpace(aggregateIdValue);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var input = $"{aggregateIdType}:{aggregateIdValue}:{userId}";
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = HMACSHA256.HashData(keyBytes, inputBytes);

        var adjectiveIndex = GetIndexFromHash(hashBytes, 0, PseudonymWordLists.Adjectives.Count);
        var nounIndex = GetIndexFromHash(hashBytes, 4, PseudonymWordLists.Nouns.Count);
        var thirdWordIndex = GetIndexFromHash(hashBytes, 8, PseudonymWordLists.CombinedThirdWords.Count);

        var adjective = PseudonymWordLists.Adjectives[adjectiveIndex];
        var noun = PseudonymWordLists.Nouns[nounIndex];
        var thirdWord = PseudonymWordLists.CombinedThirdWords[thirdWordIndex];

        return $"{adjective}{Separator}{noun}{Separator}{thirdWord}";
    }

    /// <summary>
    /// Extracts a deterministic index from hash bytes using modulo arithmetic.
    /// </summary>
    /// <param name="hashBytes">The hash bytes to extract from.</param>
    /// <param name="offset">The offset in the hash to start reading.</param>
    /// <param name="maxValue">The maximum value (exclusive) for the index.</param>
    /// <returns>An index in the range [0, maxValue).</returns>
    private static int GetIndexFromHash(byte[] hashBytes, int offset, int maxValue)
    {
        var value = BitConverter.ToUInt32(hashBytes, offset);
        return (int)(value % (uint)maxValue);
    }
}