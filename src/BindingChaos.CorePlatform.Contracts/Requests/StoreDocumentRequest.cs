namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for storing a document.
/// </summary>
/// <param name="FileName">The name of the document, including its extension.</param>
/// <param name="ContentType">The MIME type of the document, such as "application/pdf" or "image/png".</param>
/// <param name="SizeBytes">The size of the document in bytes.</param>
public record StoreDocumentRequest(
    string FileName,
    string ContentType,
    long SizeBytes);
