namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Strongly-typed options for Core Platform integration.
/// </summary>
public sealed class CorePlatformOptions
{
    /// <summary>
    /// The base address of the Core Platform (e.g., https://localhost:5001).
    /// </summary>
    public string? BaseAddress { get; set; }
}
