using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Services;
using BindingChaos.CorePlatform.Contracts.Models;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.CorePlatform.API.Controllers;

/// <summary>
/// Controller for managing documents in the Core Platform API.
/// </summary>
/// <param name="documentManagementService">The document management service.</param>
/// <param name="logger">The logger instance.</param>
[ApiController]
[Route("api/documents")]
public sealed partial class DocumentsController(IDocumentManagementService documentManagementService, ILogger<DocumentsController> logger) : BaseApiController
{
    private readonly IDocumentManagementService _documentManagementService = documentManagementService ?? throw new ArgumentNullException(nameof(documentManagementService));
    private readonly ILogger<DocumentsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Stores a document in the system.
    /// </summary>
    /// <param name="file">The uploaded file containing the document content.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The unique identifier of the stored document.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("storeDocument")]
    public async Task<IActionResult> StoreDocument([FromForm] IFormFile file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        var participantId = this.HttpContext.GetParticipantIdOrAnonymous();
        if (file.Length == 0)
        {
            return BadRequest("File cannot be empty.");
        }

        Logs.LogDocumentUploadStarted(_logger, file.FileName, file.Length);

        try
        {
            var documentId = await _documentManagementService.StoreDocumentAsync(
                file.OpenReadStream(),
                file.FileName,
                file.ContentType,
                file.Length,
                participantId.IsAnonymous ? null : participantId.Value,
                cancellationToken)
                .ConfigureAwait(false);

            Logs.LogDocumentStoredSuccessfully(_logger, documentId);

            return Created($"api/documents/{documentId}", documentId);
        }
        catch (Exception ex)
        {
            Logs.LogDocumentFailedToStore(_logger, file.FileName, ex);
            throw;
        }
    }

    /// <summary>
    /// Retrieves document metadata by its identifier.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document metadata.</returns>
    [HttpGet("{documentId}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentMetadata>), 200)]
    [ProducesResponseType(404)]
    [EndpointName("getDocumentMetadata")]
    public async Task<IActionResult> GetDocumentMetadata(string documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var metadata = await _documentManagementService.GetDocumentMetadataAsync(documentId, cancellationToken).ConfigureAwait(false);

            return Ok(metadata);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Document with ID '{documentId}' not found.");
        }
    }

    /// <summary>
    /// Downloads a document by its identifier.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document content stream.</returns>
    [HttpGet("{documentId}/content")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [EndpointName("downloadDocument")]
    public async Task<IActionResult> DownloadDocument(string documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var metadata = await _documentManagementService.GetDocumentMetadataAsync(documentId, cancellationToken).ConfigureAwait(false);
            var contentStream = await _documentManagementService.GetDocumentContentAsync(documentId, cancellationToken).ConfigureAwait(false);

            return File(contentStream, metadata.ContentType, metadata.FileName);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Document with ID '{documentId}' not found.");
        }
    }

    /// <summary>
    /// Deletes a document by its identifier.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content on successful deletion.</returns>
    [HttpDelete("{documentId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [EndpointName("deleteDocument")]
    public async Task<IActionResult> DeleteDocument(string documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _documentManagementService.DeleteDocumentAsync(documentId, cancellationToken).ConfigureAwait(false);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Document with ID '{documentId}' not found.");
        }
    }

    private static partial class Logs
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Starting document upload for file: {FileName}, Size: {FileSize} bytes")]
        internal static partial void LogDocumentUploadStarted(ILogger logger, string fileName, long fileSize);

        [LoggerMessage(Level = LogLevel.Information, Message = "Document stored successfully with ID: {DocumentId}")]
        internal static partial void LogDocumentStoredSuccessfully(ILogger logger, string documentId);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to store document {FileName}")]
        internal static partial void LogDocumentFailedToStore(ILogger logger, string fileName, Exception? ex);
    }
}
