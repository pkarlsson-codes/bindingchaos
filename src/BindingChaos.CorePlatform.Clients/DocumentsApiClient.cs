using System.Net.Http.Headers;
using BindingChaos.CorePlatform.Contracts.Models;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of the Documents API client.
/// </summary>
/// <param name="httpClient">The <see cref="HttpClient"/> instance used to send HTTP requests.</param>
/// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance used to log diagnostic messages.</param>
public partial class DocumentsApiClient(
    HttpClient httpClient,
    ILogger<DocumentsApiClient> logger)
    : BaseApiClient(httpClient, logger), IDocumentsApiClient
{
    /// <inheritdoc/>
    public async Task<string> StoreDocumentAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);

        using var formData = new MultipartFormDataContent();

        var streamContent = new StreamContent(file.OpenReadStream());
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        formData.Add(streamContent, "file", file.FileName);

        return await PostFormAsync<string>("api/documents", formData, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Stream> GetDocumentContentAsync(
        string documentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(documentId);

        return await GetStreamAsync($"api/documents/{documentId}/content", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<DocumentMetadata> GetDocumentMetadataAsync(
        string documentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(documentId);

        return GetAsync<DocumentMetadata>($"api/documents/{documentId}", cancellationToken);
    }
}
