using System.Text.Json;
using BindingChaos.Web.Gateway.Configuration;
using BindingChaos.Web.Gateway.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// OIDC endpoints for initiating login and handling the authorization code callback.
/// Implements Auth Code + PKCE and issues server-side session cookie.
/// </summary>
[ApiController]
[Route("auth")] // Matches redirect URI base
public sealed class OidcController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ITokenStore _tokenStore;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHostEnvironment _env;

    /// <summary>
    /// Initializes a new instance of the <see cref="OidcController"/> class.
    /// </summary>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="tokenStore">Token store for managing server-side tokens.</param>
    /// <param name="httpClientFactory">Factory for creating HTTP clients.</param>
    /// <param name="env">Host environment used to determine cookie security flags.</param>
    public OidcController(IConfiguration configuration, ITokenStore tokenStore, IHttpClientFactory httpClientFactory, IHostEnvironment env)
    {
        _configuration = configuration;
        _tokenStore = tokenStore;
        _httpClientFactory = httpClientFactory;
        _env = env;
    }

    /// <summary>
    /// Initiates the OIDC authorization request with PKCE and state.
    /// </summary>
    /// <returns>Redirect to the identity provider's authorize endpoint.</returns>
    [HttpGet("login")]
    public IActionResult Login()
    {
        var requestedReturnUrl = HttpContext.Request.Query["returnUrl"].ToString();
        var defaultRedirect = _configuration["Authentication:OIDC:PostLoginRedirect"] ?? "/";

        var redirectTarget = Uri.TryCreate(requestedReturnUrl, UriKind.Absolute, out _)
            ? requestedReturnUrl
            : defaultRedirect;

        var props = new AuthenticationProperties
        {
            RedirectUri = redirectTarget,
        };
        return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Initiates the OIDC authorization request with <c>prompt=create</c> to open the registration UI directly.
    /// </summary>
    /// <returns>Redirect to the identity provider's registration screen.</returns>
    [HttpGet("register")]
    public IActionResult Register()
    {
        var requestedReturnUrl = HttpContext.Request.Query["returnUrl"].ToString();
        var defaultRedirect = _configuration["Authentication:OIDC:PostLoginRedirect"] ?? "/";

        var redirectTarget = Uri.TryCreate(requestedReturnUrl, UriKind.Absolute, out _)
            ? requestedReturnUrl
            : defaultRedirect;

        var props = new AuthenticationProperties
        {
            RedirectUri = redirectTarget,
        };
        props.SetParameter("prompt", "create");
        return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Signs the user out of both cookie and OIDC schemes, clears server-side tokens, and redirects to the app.
    /// </summary>
    /// <returns>Redirect to post-logout location.</returns>
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        var sessionId = Request.Cookies[GatewayDefaults.Cookies.SessionCookie];
        string? idToken = null;
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            var tokens = await _tokenStore.TryGetTokensAsync(sessionId, HttpContext.RequestAborted).ConfigureAwait(false);
            idToken = tokens?.idToken;
            await _tokenStore.RemoveAsync(sessionId, HttpContext.RequestAborted).ConfigureAwait(false);
            Response.Cookies.Delete(GatewayDefaults.Cookies.SessionCookie);
            Response.Cookies.Delete(GatewayDefaults.Cookies.CsrfCookie);
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = _configuration["Authentication:OIDC:PostLogoutRedirect"]
                          ?? _configuration["Authentication:OIDC:PostLoginRedirect"]
                          ?? "/",
        };

        // Keycloak requires id_token_hint on the end_session endpoint.
        // The OIDC middleware reads ".Token.id_token" from AuthenticationProperties to build this parameter.
        if (!string.IsNullOrEmpty(idToken))
        {
            props.Items[".Token.id_token"] = idToken;
        }

        return SignOut(props, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Exchanges a refresh token for a new access token using the server-side token store.
    /// </summary>
    /// <returns>200 on success; 401 if session/refresh token missing; 502 on IdP error.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var sessionId = Request.Cookies[GatewayDefaults.Cookies.SessionCookie];
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Unauthorized(new { error = "No session" });
        }

        var tokens = await _tokenStore.TryGetTokensAsync(sessionId, HttpContext.RequestAborted).ConfigureAwait(false);
        if (tokens is null || string.IsNullOrWhiteSpace(tokens.Value.refreshToken))
        {
            return Unauthorized(new { error = "No refresh token" });
        }

        var authority = _configuration["Authentication:OIDC:Authority"]?.TrimEnd('/');
        var clientId = _configuration["Authentication:OIDC:ClientId"];
        var clientSecret = _configuration["Authentication:OIDC:ClientSecret"];
        if (string.IsNullOrWhiteSpace(authority) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            return StatusCode(500, new { error = "OIDC configuration incomplete" });
        }

        var tokenEndpoint = $"{authority}/protocol/openid-connect/token";
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = clientId!,
            ["client_secret"] = clientSecret!,
            ["refresh_token"] = tokens.Value.refreshToken!,
        };

        var http = _httpClientFactory.CreateClient();
        using var content = new FormUrlEncodedContent(form);
        var response = await http.PostAsync(tokenEndpoint, content).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(502, new { error = "Token endpoint error", status = (int)response.StatusCode });
        }

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var newAccess = root.GetProperty("access_token").GetString();
        var newRefresh = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : tokens.Value.refreshToken;
        var existingIdToken = tokens.Value.idToken;

        var newSessionId = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        await _tokenStore.RemoveAsync(sessionId, HttpContext.RequestAborted).ConfigureAwait(false);
        await _tokenStore.SaveTokensAsync(newSessionId, tokens.Value.userId, newAccess, newRefresh, existingIdToken, HttpContext.RequestAborted).ConfigureAwait(false);

        var isProd = _env.IsProduction();
        Response.Cookies.Append(
            GatewayDefaults.Cookies.SessionCookie,
            newSessionId,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = isProd,
                Path = "/",
                MaxAge = TimeSpan.FromDays(7),
            });

        var csrfToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        Response.Cookies.Append(
            GatewayDefaults.Cookies.CsrfCookie,
            csrfToken,
            new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Lax,
                Secure = isProd,
                Path = "/",
                MaxAge = TimeSpan.FromDays(7),
            });

        return Ok(new { success = true });
    }
}
