using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BindingChaos.SharedKernel.Domain;
using Microsoft.IdentityModel.Tokens;

namespace BindingChaos.CorePlatform.API.IntegrationTests;

/// <summary>
/// Mints signed JWTs for use in integration tests.
///
/// The issuer, audience, and signing key match what ApiFactory configures the API to accept,
/// so any token produced here will pass the API's JWT validation.
/// </summary>
public static class TestJwtFactory
{
    public static string CreateToken(ParticipantId participantId, string[]? roles = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiFactory.JwtSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("participant_id", participantId.Value),
            new(JwtRegisteredClaimNames.Sub, participantId.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (roles != null)
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: ApiFactory.JwtIssuer,
            audience: ApiFactory.JwtAudience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Adds a valid Bearer token to the client's default request headers.
    /// Returns the same client for fluent chaining.
    /// </summary>
    public static HttpClient WithAuthToken(this HttpClient client, string? participantId = null)
    {
        var actorId = participantId == null ? ParticipantId.Generate() : ParticipantId.Create(participantId);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateToken(actorId));
        return client;
    }
}
