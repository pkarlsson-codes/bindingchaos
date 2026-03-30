namespace BindingChaos.SignalProcessing;

/// <summary>
/// Generates text embeddings via the Text Embeddings Inference service.
/// </summary>
public interface ITeiClient
{
    /// <summary>
    /// Returns a vector embedding for the given text.
    /// </summary>
    /// <param name="text">The text to embed.</param>
    Task<float[]> GetEmbeddingAsync(string text);
}
