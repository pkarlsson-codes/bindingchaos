using System.Security.Claims;
using System.Security.Cryptography;
using BindingChaos.Web.Gateway.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BindingChaos.Web.Gateway.Services;

/// <summary>
/// Factory for creating OpenID Connect event handlers used by the gateway.
/// </summary>
public sealed class OidcEventsFactory(
    IHttpClientFactory httpFactory,
    ITokenStore tokenStore,
    IHostEnvironment env)
{
    private const string ProviderKeycloak = "keycloak";

    /// <summary>
    /// Creates the <see cref="OpenIdConnectEvents"/> configured for the gateway.
    /// </summary>
    /// <returns>Configured OpenID Connect events.</returns>
    public OpenIdConnectEvents Create()
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

    private static void AddUserIdClaim(TokenValidatedContext ctx, string userId)
    {
        var identity = (ClaimsIdentity)ctx.Principal!.Identity!;
        identity.AddClaim(new Claim("userId", userId));
    }

    /// <summary>
    /// Handles the OIDC <c>token validated</c> event.
    /// </summary>
    /// <param name="ctx">The token validated context provided by the OIDC middleware.</param>
    /// <returns>A task that completes when processing finishes.</returns>
    private async Task HandleTokenValidatedAsync(TokenValidatedContext ctx)
    {
        var http = httpFactory.CreateClient(HttpClientNames.CorePlatformSystem);

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
        await PersistTokensAndIssueCookiesAsync(ctx, userId).ConfigureAwait(false);
    }

    /// <summary>
    /// Persists provider tokens server-side and issues the session and CSRF cookies.
    /// </summary>
    /// <param name="ctx">The token validated context.</param>
    /// <param name="userId">The internal user id associated with the session.</param>
    private async Task PersistTokensAndIssueCookiesAsync(TokenValidatedContext ctx, string userId)
    {
        var accessToken = ctx.TokenEndpointResponse?.AccessToken;
        var refreshToken = ctx.TokenEndpointResponse?.RefreshToken;
        var idToken = ctx.TokenEndpointResponse?.IdToken;

        var sessionId = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        await tokenStore.SaveTokensAsync(sessionId, userId, accessToken, refreshToken, idToken, ctx.HttpContext.RequestAborted).ConfigureAwait(false);

        var isProd = env.IsProduction();

        ctx.HttpContext.Response.Cookies.Append(
            GatewayDefaults.Cookies.SessionCookie,
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
            GatewayDefaults.Cookies.CsrfCookie,
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
