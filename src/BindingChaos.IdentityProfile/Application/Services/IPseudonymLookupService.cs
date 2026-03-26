namespace BindingChaos.IdentityProfile.Application.Services;

/// <summary>
/// Provides lookup of stable participant pseudonyms from the identity store.
/// </summary>
public interface IPseudonymLookupService
{
    /// <summary>
    /// Returns a mapping of userId to pseudonym for the given user IDs.
    /// User IDs with no corresponding participant record are omitted from the result.
    /// </summary>
    /// <param name="userIds">The user identifiers to resolve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A dictionary of userId to pseudonym.</returns>
    Task<IReadOnlyDictionary<string, string>> GetPseudonymsAsync(
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the pseudonym for a single user ID, or <see langword="null"/> if the participant is not found.
    /// </summary>
    /// <param name="userId">The user identifier to resolve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The pseudonym, or <see langword="null"/> if not found.</returns>
    Task<string?> GetPseudonymAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
