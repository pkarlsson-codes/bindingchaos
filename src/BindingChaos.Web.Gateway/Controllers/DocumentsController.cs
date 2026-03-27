using BindingChaos.CorePlatform.Clients;
using BindingChaos.Infrastructure.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Web.Gateway.Controllers;

/// <summary>
/// Controller for managing documents in the web gateway.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DocumentsController"/> class.
/// </remarks>
/// <param name="documentsApiClient">The API client for interacting with the documents service.</param>
[ApiController]
[Route("api/v1/documents")]
public sealed class DocumentsController(
    IDocumentsApiClient documentsApiClient)
    : BaseApiController
{
    private readonly IDocumentsApiClient _documentsApiClient = documentsApiClient ?? throw new ArgumentNullException(nameof(documentsApiClient));

    /// <summary>
    /// Uploads a document file and stores it in the system.
    /// </summary>
    /// <param name="file">The uploaded file containing the document content.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The unique identifier of the stored document.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    [EndpointName("uploadDocument")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadDocument(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length == 0)
        {
            return BadRequest("File cannot be empty.");
        }

        var documentId = await _documentsApiClient
            .StoreDocumentAsync(file, cancellationToken);

        return Created(string.Empty, documentId);
    }

    /// <summary>
    /// Gets a document in the specified size (thumbnail, display, or original).
    /// </summary>
    /// <param name="documentId">The document identifier.</param>
    /// <param name="size">The requested image size (thumbnail, display, original).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document content with appropriate headers.</returns>
    [HttpGet("{documentId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [EndpointName("getDocument")]
    public async Task<IActionResult> GetDocument(
        [FromRoute] string documentId,
        [FromQuery] string size = "original",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(documentId))
        {
            return BadRequest("Document ID is required");
        }

        var validSizes = new[] { "thumbnail", "display", "original" };
        if (!validSizes.Contains(size.ToLowerInvariant()))
        {
            return BadRequest($"Invalid size parameter. Must be one of: {string.Join(", ", validSizes)}");
        }

        var documentStream = await _documentsApiClient
            .GetDocumentContentAsync(documentId, cancellationToken);
        var documentMetadata = await _documentsApiClient
            .GetDocumentMetadataAsync(documentId, cancellationToken);

        if (documentStream == null)
        {
            return NotFound($"Document with ID '{documentId}' not found");
        }

        var response = File(documentStream, documentMetadata.ContentType ?? "application/octet-stream");
        Response.Headers.CacheControl = "public, max-age=86400";
        Response.Headers.ETag = $"\"{documentId}\"";

        return response;
    }

    /// <summary>
    /// Gets document thumbnail specifically.
    /// Convenience endpoint for easier frontend URL construction.
    /// </summary>
    /// <param name="documentId">The document identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document thumbnail.</returns>
    [HttpGet("{documentId}/thumbnail")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [EndpointName("getDocumentThumbnail")]
    public Task<IActionResult> GetDocumentThumbnail(
        [FromRoute] string documentId,
        CancellationToken cancellationToken)
    {
        return GetDocument(documentId, "thumbnail", cancellationToken);
    }

    /// <summary>
    /// Gets document in display size specifically.
    /// Convenience endpoint for easier frontend URL construction.
    /// </summary>
    /// <param name="documentId">The document identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The document in display size.</returns>
    [HttpGet("{documentId}/display")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [EndpointName("getDocumentDisplay")]
    public Task<IActionResult> GetDocumentDisplay(
        [FromRoute] string documentId,
        CancellationToken cancellationToken)
    {
        return GetDocument(documentId, "display", cancellationToken);
    }
}
