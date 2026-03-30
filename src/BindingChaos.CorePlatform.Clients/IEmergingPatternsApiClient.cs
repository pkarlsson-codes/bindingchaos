using BindingChaos.CorePlatform.Contracts.Responses;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client interface for interacting with the Emerging Patterns API.
/// </summary>
public interface IEmergingPatternsApiClient
{
    /// <summary>
    /// Gets all currently identified emerging signal patterns.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A feed of all emerging patterns, ordered by cluster label.</returns>
    Task<EmergingPatternsResponse> GetEmergingPatternsAsync(
        CancellationToken cancellationToken);
}
