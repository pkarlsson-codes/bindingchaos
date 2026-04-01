namespace BindingChaos.IdentityProfile.Infrastructure.Utilities;

/// <summary>
/// Generates URL-safe base64url tokens for invite links.
/// </summary>
public static class InviteLinkTokenGenerator
{
    /// <summary>
    /// Generates a 22-character URL-safe base64url token from 16 random bytes.
    /// </summary>
    /// <returns>A 22-character base64url-encoded token.</returns>
    public static string Generate()
    {
        var tokenBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(tokenBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
