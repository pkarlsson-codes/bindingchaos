namespace BindingChaos.Web.Gateway.Services;

/// <summary>
/// Abstraction for server-side storage of tokens for BFF session management.
/// </summary>
public interface ITokenStore
{
    /// <summary>
    /// Saves tokens for a session.
    /// </summary>
    /// <param name="sessionId">Opaque session identifier.</param>
    /// <param name="userId">Stable user identifier (e.g., OIDC sub).</param>
    /// <param name="accessToken">Access token string.</param>
    /// <param name="refreshToken">Refresh token string.</param>
    /// <param name="idToken">ID token string, used as <c>id_token_hint</c> during logout.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveTokensAsync(string sessionId, string userId, string? accessToken, string? refreshToken, string? idToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves tokens for a session if they exist.
    /// </summary>
    /// <param name="sessionId">Session identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Tuple of userId, accessToken, refreshToken, idToken; or null if not found.</returns>
    Task<(string userId, string? accessToken, string? refreshToken, string? idToken)?> TryGetTokensAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes tokens for the given session.
    /// </summary>
    /// <param name="sessionId">Session identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(string sessionId, CancellationToken cancellationToken = default);
}
