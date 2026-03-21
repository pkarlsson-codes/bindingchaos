using BindingChaos.CorePlatform.API.Services;
using BindingChaos.CorePlatform.Contracts.Models;

namespace BindingChaos.CorePlatform.API.IntegrationTests;

/// <summary>No-op document management service used in integration tests.</summary>
internal sealed class NullDocumentManagementService : IDocumentManagementService
{
    /// <inheritdoc/>
    public Task<string> StoreDocumentAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long sizeBytes,
        string? createdBy,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Guid.NewGuid().ToString());

    /// <inheritdoc/>
    public Task<DocumentMetadata> GetDocumentMetadataAsync(
        string documentId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("Document management is not available in integration tests.");

    /// <inheritdoc/>
    public Task<Stream> GetDocumentContentAsync(
        string documentId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("Document management is not available in integration tests.");

    /// <inheritdoc/>
    public Task DeleteDocumentAsync(
        string documentId,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
