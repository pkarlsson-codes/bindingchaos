namespace BindingChaos.CorePlatform.Contracts.Models;

/// <summary>
/// Represents metadata for a document in the system.
/// </summary>
public sealed class DocumentMetadata
{
    /// <summary>
    /// Initializes a new instance of the DocumentMetadata class.
    /// </summary>
    /// <param name="id">The unique identifier of the document.</param>
    /// <param name="fileName">The name of the document file.</param>
    /// <param name="contentType">The MIME type of the document.</param>
    /// <param name="sizeBytes">The size of the document in bytes.</param>
    /// <param name="createdAt">When the document was created.</param>
    /// <param name="createdBy">Who created the document.</param>
    public DocumentMetadata(
        string id,
        string fileName,
        string contentType,
        long sizeBytes,
        DateTimeOffset createdAt,
        string? createdBy = null)
    {
        Id = id;
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Gets the unique identifier of the document.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the name of the document file.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the MIME type of the document.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// Gets the size of the document in bytes.
    /// </summary>
    public long SizeBytes { get; }

    /// <summary>
    /// Gets when the document was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets who created the document.
    /// </summary>
    public string? CreatedBy { get; }

    /// <summary>
    /// Gets a value indicating whether this document represents an image file.
    /// </summary>
    public bool IsImage => ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a value indicating whether this document represents a PDF file.
    /// </summary>
    public bool IsPdf => ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a value indicating whether this document represents a text file.
    /// </summary>
    public bool IsText => ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase);
}