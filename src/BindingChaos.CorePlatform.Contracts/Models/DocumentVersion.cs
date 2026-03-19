namespace BindingChaos.CorePlatform.Contracts.Models;

/// <summary>
/// Represents a version of a document.
/// </summary>
public sealed class DocumentVersion
{
    /// <summary>
    /// Initializes a new instance of the DocumentVersion class.
    /// </summary>
    /// <param name="versionNumber">The version number.</param>
    /// <param name="documentId">The document identifier this version belongs to.</param>
    /// <param name="fileName">The name of the document file.</param>
    /// <param name="contentType">The MIME type of the document.</param>
    /// <param name="sizeBytes">The size of the document in bytes.</param>
    /// <param name="createdAt">When this version was created.</param>
    /// <param name="createdBy">Who created this version.</param>
    /// <param name="comment">Optional comment for this version.</param>
    /// <param name="isCurrentVersion">Whether this is the current version.</param>
    public DocumentVersion(
        string versionNumber,
        string documentId,
        string fileName,
        string contentType,
        long sizeBytes,
        DateTimeOffset createdAt,
        string? createdBy = null,
        string? comment = null,
        bool isCurrentVersion = false)
    {
        VersionNumber = versionNumber;
        DocumentId = documentId;
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        Comment = comment;
        IsCurrentVersion = isCurrentVersion;
    }

    /// <summary>
    /// Gets the version number (e.g., "1.0", "2.1").
    /// </summary>
    public string VersionNumber { get; }

    /// <summary>
    /// Gets the document identifier this version belongs to.
    /// </summary>
    public string DocumentId { get; }

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
    /// Gets when this version was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets who created this version.
    /// </summary>
    public string? CreatedBy { get; }

    /// <summary>
    /// Gets the optional comment for this version.
    /// </summary>
    public string? Comment { get; }

    /// <summary>
    /// Gets a value indicating whether this is the current version.
    /// </summary>
    public bool IsCurrentVersion { get; }
}