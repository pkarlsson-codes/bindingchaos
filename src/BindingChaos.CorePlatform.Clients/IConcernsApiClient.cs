using BindingChaos.CorePlatform.Contracts.Requests;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client responsible for interacting with the concerns API endpoints.
/// </summary>
public interface IConcernsApiClient
{
    /// <summary>
    /// Raises a new concern by sending a request to the concerns API.
    /// </summary>
    /// <param name="request">The request containing the details of the concern to raise.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the raised concern as a string.</returns>
    public Task<string> RaiseConcernAsync(RaiseConcernRequest request, CancellationToken cancellationToken = default);
}
