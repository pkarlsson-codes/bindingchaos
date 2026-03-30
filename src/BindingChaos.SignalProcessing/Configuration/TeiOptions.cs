namespace BindingChaos.SignalProcessing.Configuration;

/// <summary>
/// Strongly-typed options for the Text Embeddings Inference service.
/// </summary>
public sealed class TeiOptions
{
    /// <summary>The base URL of the Text Embeddings Inference (TEI) service.</summary>
    public string BaseUrl { get; set; } = string.Empty;
}
