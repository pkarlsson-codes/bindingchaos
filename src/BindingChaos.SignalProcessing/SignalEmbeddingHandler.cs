using BindingChaos.Stigmergy.Contracts;

namespace BindingChaos.SignalProcessing;

/// <summary>
/// Handles <see cref="SignalCapturedIntegrationEvent"/> by generating a text embedding via TEI
/// and persisting it to the signal_processing.signal_embeddings table.
/// </summary>
public sealed class SignalEmbeddingHandler(
    ITeiClient teiClient,
    ISignalEmbeddingRepository embeddingRepository)
{
    /// <summary>
    /// Generates and stores an embedding for a captured signal.
    /// </summary>
    /// <param name="message">The integration event.</param>
    public async Task Handle(SignalCapturedIntegrationEvent message)
    {
        var text = BuildEmbeddingText(message);
        var embedding = await teiClient.GetEmbeddingAsync(text).ConfigureAwait(false);
        await embeddingRepository.UpsertAsync(message.SignalId, embedding).ConfigureAwait(false);
    }

    private static string BuildEmbeddingText(SignalCapturedIntegrationEvent message)
    {
        if (message.Tags.Count == 0)
            return message.Description;

        return $"{message.Description} {string.Join(" ", message.Tags)}";
    }
}
