using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BindingChaos.Web.Gateway.Services;

public interface IInternalJwtService
{
    string GenerateToken(string? participantId, bool? personhoodVerified, string? trustLevel, string? sessionId);

    /// <summary>
    /// Generates a short-lived service token used by the BFF to call downstream services.
    /// No end-user identity is asserted; optionally includes the current session id.
    /// </summary>
    /// <param name="sessionId">Optional session identifier to correlate calls.</param>
    /// <returns>Signed JWT string.</returns>
    string GenerateServiceToken(string? sessionId = null);
}

public sealed class InternalJwtService : IInternalJwtService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly SigningCredentials _credentials;
    private readonly TimeSpan _lifetime;

    public InternalJwtService(IConfiguration configuration, IHostEnvironment environment)
    {
        _issuer = configuration["InternalJwt:Issuer"] ?? "BindingChaos.Gateway";
        _audience = configuration["InternalJwt:Audience"] ?? "BindingChaos.CorePlatform";

        var secret = configuration["InternalJwt:SigningKey"]
                     ?? Environment.GetEnvironmentVariable("BINDINGCHAOS_INTERNAL_JWT_SECRET")
                     ?? (environment.IsDevelopment() ? "dev-internal-jwt-secret-change-me" : null);

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("Internal JWT signing key is not configured.");
        }

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        _credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        _lifetime = TimeSpan.FromMinutes(5);
    }

    public string GenerateToken(string? participantId, bool? personhoodVerified, string? trustLevel, string? sessionId)
    {
        var now = DateTimeOffset.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Iss, _issuer),
            new(JwtRegisteredClaimNames.Aud, _audience),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Nbf, now.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Exp, now.Add(_lifetime).ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        var subject = participantId ?? "anonymous";
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, subject));

        if (!string.IsNullOrWhiteSpace(participantId))
        {
            claims.Add(new Claim("participant_id", participantId));
        }

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            claims.Add(new Claim("session_id", sessionId));
        }

        if (personhoodVerified.HasValue)
        {
            claims.Add(new Claim("personhoodVerified", personhoodVerified.Value ? "true" : "false"));
        }

        if (!string.IsNullOrWhiteSpace(trustLevel))
        {
            claims.Add(new Claim("trustLevel", trustLevel));
        }

        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: now.Add(_lifetime).UtcDateTime,
            signingCredentials: _credentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public string GenerateServiceToken(string? sessionId = null)
    {
        var now = DateTimeOffset.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Iss, _issuer),
            new(JwtRegisteredClaimNames.Aud, _audience),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Nbf, now.ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Exp, now.Add(_lifetime).ToUnixTimeSeconds().ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Sub, "gateway-service"),
            new("actor", "gateway"),
        };

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            claims.Add(new Claim("session_id", sessionId));
        }

        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: now.Add(_lifetime).UtcDateTime,
            signingCredentials: _credentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
