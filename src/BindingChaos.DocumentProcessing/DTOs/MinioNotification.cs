namespace BindingChaos.DocumentProcessing.DTOs;

/// <summary>
/// A Minio notification.
/// </summary>
/// <param name="Records"></param>
public record MinioNotification(MinioRecord[] Records);