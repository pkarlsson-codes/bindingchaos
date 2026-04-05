using BindingChaos.CorePlatform.Contracts.Responses;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client responsible for interacting with the profiles API endpoints.
/// </summary>
public interface IProfilesApiClient
{
    /// <summary>
    /// Returns the public profile of a participant identified by their pseudonym, or null if not found.
    /// </summary>
    /// <param name="pseudonym">The participant's globally unique pseudonym.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The participant's profile, or null if not found.</returns>
    Task<ParticipantProfileResponse?> GetProfileAsync(
        string pseudonym,
        CancellationToken cancellationToken = default);
}
