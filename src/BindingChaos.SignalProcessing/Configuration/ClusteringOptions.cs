namespace BindingChaos.SignalProcessing.Configuration;

/// <summary>
/// Strongly-typed options for the Python clustering sidecar service.
/// </summary>
public sealed class ClusteringOptions
{
    /// <summary>The base URL of the clustering service.</summary>
    public string BaseUrl { get; set; } = string.Empty;
}
