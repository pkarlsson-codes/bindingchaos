using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BindingChaos.SignalProcessing;

/// <summary>
/// HTTP client for the Python clustering sidecar service.
/// </summary>
public sealed class ClusteringClient(IHttpClientFactory httpClientFactory) : IClusteringClient
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<SignalClusterResult>> ClusterAsync(IReadOnlyList<SignalEmbedding> embeddings)
    {
        var client = httpClientFactory.CreateClient("clustering");

        var request = new ClusterRequest(
            embeddings.Select(e => e.SignalId).ToArray(),
            embeddings.Select(e => e.Embedding).ToArray());

        var response = await client.PostAsJsonAsync("/cluster", request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ClusterResponse>().ConfigureAwait(false)
                     ?? throw new InvalidOperationException("Clustering service returned null response");

        return result.Results
            .Select(r => new SignalClusterResult(r.SignalId, r.ClusterLabel))
            .ToList();
    }

    private sealed record ClusterRequest(
        [property: JsonPropertyName("signal_ids")] string[] SignalIds,
        [property: JsonPropertyName("embeddings")] float[][] Embeddings);

    private sealed record ClusterResponse(
        [property: JsonPropertyName("results")] ClusterResultItem[] Results);

    private sealed record ClusterResultItem(
        [property: JsonPropertyName("signal_id")] string SignalId,
        [property: JsonPropertyName("cluster_label")] int ClusterLabel);
}
