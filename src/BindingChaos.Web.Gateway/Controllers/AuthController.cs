using System.Security.Cryptography;
using BindingChaos.CorePlatform.Clients;
using BindingChaos.Web.Gateway.Configuration;
using BindingChaos.Web.Gateway.Models;
using BindingChaos.Web.Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Authentication endpoints for login, logout, and retrieving the current user.
/// </summary>
/// <param name="env">Environment to check if production.</param>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController(IHostEnvironment env) : ControllerBase
{
    /// <summary>
    /// Issues a CSRF token cookie for anonymous visitors (double-submit cookie pattern).
    /// </summary>
    /// <returns>200 OK and sets the bc_csrf cookie.</returns>
    [HttpGet("csrf")]
    [EndpointName("issueCsrfToken")]
    public IActionResult IssueCsrf()
    {
        var isProd = env.IsProduction();
        var csrfToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        Response.Cookies.Append(
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
        return Ok(new { success = true });
    }

    /// <summary>
    /// Get current authenticated user.
    /// </summary>
    /// <param name="tokenStore">Token store used to resolve the current session.</param>
    /// <param name="profilesApiClient">Client for resolving the participant's pseudonym.</param>
    /// <returns>The current user if authenticated; otherwise unauthorized.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(GetCurrentUserResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [EndpointName("getCurrentUser")]
    public async Task<ActionResult<GetCurrentUserResponse>> GetCurrentUser(
        [FromServices] ITokenStore tokenStore,
        [FromServices] IProfilesApiClient profilesApiClient)
    {
        var sessionId = Request.Cookies[GatewayDefaults.Cookies.SessionCookie];

        if (string.IsNullOrEmpty(sessionId))
        {
            return Unauthorized(new GetCurrentUserResponse
            {
                Success = false,
                Error = "No session found",
            });
        }

        var tokens = await tokenStore
            .TryGetTokensAsync(sessionId, HttpContext.RequestAborted);
        if (tokens is null)
        {
            return Unauthorized(new GetCurrentUserResponse
            {
                Success = false,
                Error = "Invalid or expired session",
            });
        }

        var (userId, _, _, _) = tokens.Value;

        var preferredUsername = User?.FindFirst("preferred_username")?.Value
                                 ?? User?.Identity?.Name
                                 ?? string.Empty;
        var email = User?.FindFirst("email")?.Value ?? string.Empty;

        var safeUsername = string.IsNullOrWhiteSpace(preferredUsername) ? userId : preferredUsername;
        var safeEmail = string.IsNullOrEmpty(email) ? string.Empty : email;

        var profile = await profilesApiClient
            .GetProfileByUserIdAsync(userId, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var user = new UserInfo
        {
            Id = userId,
            Username = safeUsername,
            Pseudonym = profile?.Pseudonym ?? userId,
            Email = safeEmail,
        };

        Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
        return Ok(new GetCurrentUserResponse
        {
            Success = true,
            User = user,
        });
    }
}
