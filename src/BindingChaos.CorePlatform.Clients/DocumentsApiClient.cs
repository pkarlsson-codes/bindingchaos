using System.Net;
using System.Net.Http.Headers;
using BindingChaos.CorePlatform.Contracts.Models;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BindingChaos.CorePlatform.Clients;

/// <summary>
/// Implementation of the Documents API client.
/// </summary>
public partial class DocumentsApiClient : BaseApiClient, IDocumentsApiClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentsApiClient"/> class with the specified HTTP client and
    /// logger.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> instance used to send HTTP requests.</param>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance used to log diagnostic messages.</param>
    public DocumentsApiClient(HttpClient httpClient, ILogger<DocumentsApiClient> logger)
        : base(httpClient, logger)
    {
    }

    /// <inheritdoc/>
    public async Task<string> StoreDocumentAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        using var formData = new MultipartFormDataContent();

        var streamContent = new StreamContent(file.OpenReadStream());
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        formData.Add(streamContent, "file", file.FileName);

        try
        {
            return await PostFormAsync<string>("api/documents", formData, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logs.UploadError(Logger, file.FileName, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Stream> GetDocumentContentAsync(string documentId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(documentId);

        try
        {
            var response = await HttpClient.GetAsync($"api/documents/{documentId}/content", cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            Logs.DocumentNotFound(Logger, documentId, ex);
            throw;
        }
        catch (Exception ex)
        {
            Logs.GetContentError(Logger, documentId, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DocumentMetadata> GetDocumentMetadataAsync(string documentId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(documentId);

        try
        {
            return await GetAsync<DocumentMetadata>($"api/documents/{documentId}", cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            Logs.DocumentNotFound(Logger, documentId, ex);
            throw;
        }
        catch (Exception ex)
        {
            Logs.GetMetadataError(Logger, documentId, ex);
            throw;
        }
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Failed to upload document. Status: {StatusCode}, Error: {Error}")]
        internal static partial void UploadFailed(ILogger logger, HttpStatusCode statusCode, string error);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error uploading document {FileName}")]
        internal static partial void UploadError(ILogger logger, string fileName, Exception? exception);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Document not found: {DocumentId}")]
        internal static partial void DocumentNotFound(ILogger logger, string documentId, Exception? exception);

        [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Error getting document content for {DocumentId}")]
        internal static partial void GetContentError(ILogger logger, string documentId, Exception? exception);

        [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Error getting document metadata for {DocumentId}")]
        internal static partial void GetMetadataError(ILogger logger, string documentId, Exception? exception);
    }
}
