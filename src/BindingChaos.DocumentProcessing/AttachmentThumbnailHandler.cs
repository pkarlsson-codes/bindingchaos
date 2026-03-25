using BindingChaos.DocumentProcessing.DTOs;
using Minio;
using Minio.DataModel.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Wolverine;

namespace BindingChaos.DocumentProcessing;

/// <summary>
/// Creates thumbnails for attachments.
/// </summary>
public sealed class AttachmentThumbnailHandler(IMinioClient minio)
{
    /// <summary>
    /// Processes a MinIO notification by generating a thumbnail for a JPEG image and saving it to the storage service.
    /// Publishes a notification upon successful thumbnail creation.
    /// </summary>
    /// <param name="message">The MinIO notification message containing information about the S3 object that triggered the event. Must not be
    /// null.</param>
    /// <param name="bus">The message context used to publish notifications to the Core API. Must not be null.</param>
    /// <returns>A task that represents the asynchronous operation of handling the notification.</returns>
    public async Task Handle(MinioNotification message, IMessageContext bus)
    {
        var file = message.Records.FirstOrDefault();
        if (file == null || !file.S3.Object.Key.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)) return;

        using var originalStream = new MemoryStream();
        await minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(file.S3.Bucket.Name)
            .WithObject(file.S3.Object.Key)
            .WithCallbackStream(s => s.CopyTo(originalStream)))
            .ConfigureAwait(false);

        originalStream.Position = 0;

        using var image = Image.Load(originalStream);
        image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(200, 200), Mode = ResizeMode.Max }));

        using var thumbStream = new MemoryStream();
        await image.SaveAsJpegAsync(thumbStream).ConfigureAwait(false);
        thumbStream.Position = 0;

        var thumbKey = $"thumbnails/{file.S3.Object.Key}";
        await minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket("attachments")
            .WithObject(thumbKey)
            .WithStreamData(thumbStream)
            .WithObjectSize(thumbStream.Length)
            .WithContentType("image/jpeg"))
            .ConfigureAwait(false);

        await bus.PublishAsync(new ThumbnailCreated(file.S3.Object.Key, thumbKey))
            .ConfigureAwait(false);
    }
}