using System.Security.Claims;
using System.Security.Cryptography;
using BindingChaos.Web.Gateway.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BindingChaos.Web.Gateway.Services;

/// <summary>
/// Factory for creating OpenID Connect event handlers used by the gateway.
/// </summary>
public static class OidcEventsFactory
{
    private const string ProviderKeycloak = "keycloak";
    private const string CookieNameSession = "bc_session";
    private const string CookieNameCsrf = "bc_csrf";

    /// <summary>
    /// Creates the <see cref="OpenIdConnectEvents"/> configured for the gateway.
    /// </summary>
    /// <returns>Configured OpenID Connect events.</returns>
    public static OpenIdConnectEvents Create()
    {
        return new OpenIdConnectEvents
        {
            OnTokenValidated = HandleTokenValidatedAsync,
            OnRedirectToIdentityProviderForSignOut = context =>
            {
                context.ProtocolMessage.ClientId = context.Options.ClientId;
                return Task.CompletedTask;
            },
        };
    }

    /// <summary>
    /// Handles the OIDC <c>token validated</c> event.
    /// </summary>
    /// <param name="ctx">The token validated context provided by the OIDC middleware.</param>
    /// <returns>A task that completes when processing finishes.</returns>
    private static async Task HandleTokenValidatedAsync(TokenValidatedContext ctx)
    {
        var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<OpenIdConnectEvents>>();
        var httpFactory = ctx.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
        var http = httpFactory.CreateClient(HttpClientNames.CorePlatformSystem);
        var tokenStore = ctx.HttpContext.RequestServices.GetRequiredService<ITokenStore>();

        if (!TryGetSubject(ctx.Principal, out var subject))
        {
            ctx.Fail("Missing subject (sub) claim from identity provider");
            return;
        }

        (bool linkOk, string? resolvedUserId, int? statusCode) = await LinkUserAndResolveUserIdAsync(
            http,
            ProviderKeycloak,
            subject,
            ctx.HttpContext.RequestAborted).ConfigureAwait(false);

        if (!linkOk)
        {
            ctx.Fail($"Failed to link user identity: {statusCode?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "unknown"}");
            return;
        }

        var userId = resolvedUserId ?? subject;

        AddUserIdClaim(ctx, userId);
        await PersistTokensAndIssueCookiesAsync(ctx, tokenStore, userId).ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to get the OIDC subject (<c>sub</c>) from the authenticated principal.
    /// </summary>
    /// <param name="principal">The authenticated principal.</param>
    /// <param name="subject">The extracted subject value, if found.</param>
    /// <returns><see langword="true"/> if the subject was found; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetSubject(ClaimsPrincipal? principal, out string subject)
    {
        subject = string.Empty;

        if (principal == null)
        {
            return false;
        }

        var subClaim = principal.FindFirst("sub")?.Value;
        if (!string.IsNullOrWhiteSpace(subClaim))
        {
            subject = subClaim;
            return true;
        }

        subClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(subClaim))
        {
            subject = subClaim;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the Core Platform base address from configuration.
    /// </summary>
    /// <param name="ctx">The current token validated context.</param>
    /// <returns>The trimmed Core Platform base address, or <see langword="null"/> if missing.</returns>
    private static string? GetCorePlatformBase(TokenValidatedContext ctx)
    {
        var options = ctx.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<BindingChaos.Web.Gateway.Configuration.CorePlatformOptions>>();
        return options.Value.BaseAddress?.TrimEnd('/');
    }

    /// <summary>
    /// Links the external identity to an internal user and returns the resolved user id.
    /// </summary>
    /// <param name="http">The authorized HTTP client.</param>
    /// <param name="provider">The external identity provider name.</param>
    /// <param name="subject">The OIDC subject (<c>sub</c>).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A tuple indicating success, an optional user id, and an optional HTTP status code.</returns>
    private static async Task<(bool Success, string? UserId, int? StatusCode)> LinkUserAndResolveUserIdAsync(
        HttpClient http,
        string provider,
        string subject,
        CancellationToken cancellationToken)
    {
        using var linkContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(new { Provider = provider, Subject = subject }), System.Text.Encoding.UTF8, "application/json");
        using var linkResponse = await http.PostAsync("api/identity/users/link", linkContent, cancellationToken).ConfigureAwait(false);
        if (!linkResponse.IsSuccessStatusCode)
        {
            return (false, null, (int)linkResponse.StatusCode);
        }

        var linkJson = await linkResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return (true, TryParseUserIdFromJson(linkJson), null);
    }

    /// <summary>
    /// Attempts to parse a <c>userId</c> from a JSON payload.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The extracted user id if present; otherwise, <see langword="null"/>.</returns>
    private static string? TryParseUserIdFromJson(string json)
    {
        using var linkDoc = System.Text.Json.JsonDocument.Parse(json);
        var root = linkDoc.RootElement;
        if (root.TryGetProperty("userId", out var directUserId))
        {
            return directUserId.GetString();
        }

        if (root.TryGetProperty("data", out var dataObj) &&
            dataObj.ValueKind == System.Text.Json.JsonValueKind.Object &&
            dataObj.TryGetProperty("userId", out var nestedUserId))
        {
            return nestedUserId.GetString();
        }

        return null;
    }

    /// <summary>
    /// Adds the resolved internal <c>userId</c> claim to the current principal.
    /// </summary>
    /// <param name="ctx">The token validated context containing the principal.</param>
    /// <param name="userId">The internal user identifier.</param>
    private static void AddUserIdClaim(TokenValidatedContext ctx, string userId)
    {
        var identity = (ClaimsIdentity)ctx.Principal!.Identity!;
        identity.AddClaim(new Claim("userId", userId));
    }

    /// <summary>
    /// Persists provider tokens server-side and issues the session and CSRF cookies.
    /// </summary>
    /// <param name="ctx">The token validated context.</param>
    /// <param name="tokenStore">The token store abstraction.</param>
    /// <param name="userId">The internal user id associated with the session.</param>
    private static async Task PersistTokensAndIssueCookiesAsync(TokenValidatedContext ctx, ITokenStore tokenStore, string userId)
    {
        var accessToken = ctx.TokenEndpointResponse?.AccessToken;
        var refreshToken = ctx.TokenEndpointResponse?.RefreshToken;
        var idToken = ctx.TokenEndpointResponse?.IdToken;

        var sessionId = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        await tokenStore.SaveTokensAsync(sessionId, userId, accessToken, refreshToken, idToken, ctx.HttpContext.RequestAborted).ConfigureAwait(false);

        var env = ctx.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        var isProd = env.IsProduction();

        ctx.HttpContext.Response.Cookies.Append(
            CookieNameSession,
            sessionId,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = isProd,
                Path = "/",
                MaxAge = TimeSpan.FromDays(7),
            });

        var csrfToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        ctx.HttpContext.Response.Cookies.Append(
            CookieNameCsrf,
            csrfToken,
            new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict,
                Secure = isProd,
                Path = "/",
                MaxAge = TimeSpan.FromDays(7),
            });
    }
}
