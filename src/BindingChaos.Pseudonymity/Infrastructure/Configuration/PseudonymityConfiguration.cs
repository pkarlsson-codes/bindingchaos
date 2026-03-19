namespace BindingChaos.Pseudonymity.Infrastructure.Configuration;

/// <summary>
/// Configuration options for the Pseudonymity service.
/// </summary>
public sealed class PseudonymityConfiguration
{
    /// <summary>
    /// Gets or sets the HMAC secret key used for deterministic pseudonym generation.
    /// This should be a secure random string that is kept secret.
    /// </summary>
    public string HmacSecretKey { get; set; } = string.Empty;
}
