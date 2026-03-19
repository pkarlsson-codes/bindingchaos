namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Strongly-typed options describing the Gateway's own public base URL.
/// </summary>
public sealed class GatewayOptions
{
    /// <summary>
    /// The public base URL of this gateway (e.g., http://localhost:4000).
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
}
