namespace BindingChaos.SignalProcessing;

/// <summary>
/// A stored signal embedding.
/// </summary>
/// <param name="SignalId">The signal identifier.</param>
/// <param name="Embedding">The vector embedding.</param>
public sealed record SignalEmbedding(string SignalId, float[] Embedding);

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
    Task UpsertAsync(string signalId, float[] embedding);

    /// <summary>
    /// Returns all stored signal embeddings.
    /// </summary>
    Task<IReadOnlyList<SignalEmbedding>> GetAllAsync();
}
