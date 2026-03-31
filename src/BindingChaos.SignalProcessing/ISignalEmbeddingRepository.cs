namespace BindingChaos.SignalProcessing;

/// <summary>
/// A stored signal embedding.
/// </summary>
/// <param name="SignalId">The signal identifier.</param>
/// <param name="Embedding">The vector embedding.</param>
/// <param name="SignalText">The source text that was embedded.</param>
public sealed record SignalEmbedding(string SignalId, float[] Embedding, string SignalText);

/// <summary>
/// Stores signal embeddings for later similarity and clustering queries.
/// </summary>
public interface ISignalEmbeddingRepository
{
    /// <summary>
    /// Inserts or replaces the embedding for the given signal.
    /// </summary>
    /// <param name="signalId">The signal identifier.</param>
    /// <param name="embedding">The vector embedding.</param>
    /// <param name="signalText">The source text that was embedded.</param>
    Task UpsertAsync(string signalId, float[] embedding, string signalText);

    /// <summary>
    /// Returns all stored signal embeddings.
    /// </summary>
    Task<IReadOnlyList<SignalEmbedding>> GetAllAsync();
}
