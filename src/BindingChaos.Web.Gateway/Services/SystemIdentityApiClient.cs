namespace BindingChaos.Web.Gateway.Services;

/// <summary>
/// Client for Core Platform identity operations used by the gateway.
/// </summary>
public interface ISystemIdentityApiClient
{
    /// <summary>
    /// Links an external identity (provider + subject) to an internal user and returns the user id.
    /// </summary>
    /// <param name="provider">External identity provider (e.g., keycloak).</param>
    /// <param name="subject">Provider subject (OIDC sub).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A tuple indicating success, resolved user id if any, and failing status code if any.</returns>
    Task<(bool Success, string? UserId, int? StatusCode)> LinkUserAsync(string provider, string subject, CancellationToken cancellationToken);
}

/// <summary>
/// Default implementation of <see cref="ISystemIdentityApiClient"/> using a typed <see cref="HttpClient"/>.
/// </summary>
public sealed class SystemIdentityApiClient : ISystemIdentityApiClient
{
    private readonly HttpClient _httpClient;

    public SystemIdentityApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(bool Success, string? UserId, int? StatusCode)> LinkUserAsync(string provider, string subject, CancellationToken cancellationToken)
    {
        var requestBody = new { Provider = provider, Subject = subject };
        using var response = await _httpClient.PostAsJsonAsync("api/identity/users/link", requestBody, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return (false, null, (int)response.StatusCode);
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return (true, TryParseUserIdFromJson(json), null);
    }

    private static string? TryParseUserIdFromJson(string json)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("userId", out var direct))
        {
            return direct.GetString();
        }

        if (root.TryGetProperty("data", out var data) && data.ValueKind == System.Text.Json.JsonValueKind.Object && data.TryGetProperty("userId", out var nested))
        {
            return nested.GetString();
        }

        return null;
    }
}
