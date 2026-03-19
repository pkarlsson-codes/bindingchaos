using BindingChaos.CorePlatform.Contracts.Models;

namespace BindingChaos.CorePlatform.API.Services;

/// <summary>
/// Implementation-agnostic document management service interface.
/// Provides comprehensive document operations following ECM standards.
/// </summary>
public interface IDocumentManagementService
{
    /// <summary>
    /// Stores a document in the system and returns a unique identifier.
    /// </summary>
    /// <param name="fileStream">The stream containing the document data.</param>
    /// <param name="fileName">The name of the document including extension.</param>
    /// <param name="contentType">The MIME type of the document.</param>
    /// <param name="sizeBytes">The size of the document in bytes.</param>
    /// <param name="createdBy">Id of the users that uploaded the document.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The unique identifier of the stored document.</returns>
    Task<string> StoreDocumentAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long sizeBytes,
        string? createdBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves document metadata by its identifier.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Document metadata including file information and properties.</returns>
    Task<DocumentMetadata> GetDocumentMetadataAsync(
        string documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the content stream of a document.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A stream containing the document content.</returns>
    Task<Stream> GetDocumentContentAsync(
        string documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document from the system.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteDocumentAsync(
        string documentId,
        CancellationToken cancellationToken = default);
}
