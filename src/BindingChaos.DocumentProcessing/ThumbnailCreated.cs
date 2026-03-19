namespace BindingChaos.DocumentProcessing;

/// <summary>
/// Event representing that a thumbnail was created.
/// </summary>
/// <param name="OriginalId"></param>
/// <param name="ThumbnailId"></param>
public record ThumbnailCreated(string OriginalId, string ThumbnailId);