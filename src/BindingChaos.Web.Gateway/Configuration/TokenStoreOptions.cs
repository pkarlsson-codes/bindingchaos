namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Strongly-typed options for the Redis-backed token store.
/// </summary>
public sealed class TokenStoreOptions
{
    /// <summary>
    /// How long tokens are retained in Redis. Defaults to 7 days.
    /// </summary>
    public int DefaultTtlMinutes { get; set; } = 60 * 24 * 7;
}
