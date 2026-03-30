using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BindingChaos.SignalProcessing;

/// <summary>
/// HTTP client for the Text Embeddings Inference service.
/// </summary>
public sealed class TeiClient(IHttpClientFactory httpClientFactory) : ITeiClient
{
    /// <inheritdoc />
    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var client = httpClientFactory.CreateClient("tei");
        var response = await client.PostAsJsonAsync("/embed", new EmbedRequest([text])).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<float[][]>().ConfigureAwait(false)
                     ?? throw new InvalidOperationException("TEI returned null response");
        return result[0];
    }

    private sealed record EmbedRequest(
        [property: JsonPropertyName("inputs")] string[] Inputs);
}
