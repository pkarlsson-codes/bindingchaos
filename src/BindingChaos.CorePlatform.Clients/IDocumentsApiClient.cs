using BindingChaos.CorePlatform.Contracts.Models;
using Microsoft.AspNetCore.Http;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Client interface for the Documents API.
/// </summary>
public interface IDocumentsApiClient
{
    /// <summary>
    /// Stores a document in the system.
    /// </summary>
    /// <param name="file">The file to store.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The unique identifier of the stored document.</returns>
    Task<string> StoreDocumentAsync(IFormFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets document content as a stream.
    /// </summary>
    /// <param name="documentId">The document identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document content stream.</returns>
    Task<Stream> GetDocumentContentAsync(string documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets document metadata.
    /// </summary>
    /// <param name="documentId">The document identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document metadata.</returns>
    Task<DocumentMetadata> GetDocumentMetadataAsync(string documentId, CancellationToken cancellationToken = default);
}
