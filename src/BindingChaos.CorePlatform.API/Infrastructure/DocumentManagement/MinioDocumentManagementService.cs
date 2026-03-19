using System.Globalization;
using BindingChaos.CorePlatform.API.Services;
using BindingChaos.CorePlatform.Contracts.Models;
using Minio;
using Minio.DataModel.Args;

namespace BindingChaos.CorePlatform.API.Infrastructure.DocumentManagement;

/// <summary>
/// Provides an implementation of the IDocumentManagementService interface for managing documents using a MinIO-based
/// storage backend. Supports asynchronous operations for storing, retrieving, and deleting documents.
/// </summary>
internal class MinioDocumentManagementService : IDocumentManagementService
{
    private const string AttachmentsBucket = "attachments";
    private readonly IMinioClient _minio;
    private readonly Task _ensureBucketTask;

    /// <summary>
    /// Instantiates a new instance of the <see cref="MinioDocumentManagementService"/> class.
    /// </summary>
    /// <param name="minio">The Minio client.</param>
    public MinioDocumentManagementService(IMinioClient minio)
    {
        _minio = minio;
        _ensureBucketTask = EnsureBucketExistsAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteDocumentAsync(string documentId, CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(AttachmentsBucket)
            .WithObject(documentId);

        await _minio.RemoveObjectAsync(args, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Stream> GetDocumentContentAsync(string documentId, CancellationToken cancellationToken = default)
    {
        var outputStream = new MemoryStream();

        await _ensureBucketTask.ConfigureAwait(false);

        await _minio.GetObjectAsync(
            new GetObjectArgs()
                .WithBucket(AttachmentsBucket)
                .WithObject(documentId)
                .WithCallbackStream((stream) =>
                {
                    stream.CopyTo(outputStream);
                }),
            cancellationToken)
            .ConfigureAwait(false);

        outputStream.Position = 0; // Reset pointer for the consumer
        return outputStream;
    }

    /// <inheritdoc/>
    public async Task<DocumentMetadata> GetDocumentMetadataAsync(string documentId, CancellationToken cancellationToken = default)
    {
        await _ensureBucketTask.ConfigureAwait(false);

        var args = new StatObjectArgs()
            .WithBucket(AttachmentsBucket)
            .WithObject(documentId);

        var stat = await _minio.StatObjectAsync(args, cancellationToken).ConfigureAwait(false);

        return new DocumentMetadata(
            id: documentId,
            fileName: documentId.Split('_', 2).LastOrDefault() ?? documentId, // Extract original name if possible
            contentType: stat.ContentType,
            sizeBytes: stat.Size,
            createdAt: stat.LastModified,
            createdBy: "MinIO-System");
    }

    /// <inheritdoc/>
    public async Task<string> StoreDocumentAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            long sizeBytes,
            string? createdBy,
            CancellationToken cancellationToken = default)
    {
        string uniqueId = Guid.NewGuid().ToString();

        var metaData = new Dictionary<string, string>
        {
            { MetaKeys.OriginalName, fileName },
            { MetaKeys.CreatedAt, DateTime.UtcNow.ToString("U", CultureInfo.InvariantCulture) },
        };

        if (createdBy is not null)
        {
            metaData.Add(MetaKeys.CreatedBy, createdBy);
        }

        await _ensureBucketTask.ConfigureAwait(false);

        var putArgs = new PutObjectArgs()
            .WithBucket(AttachmentsBucket)
            .WithObject(uniqueId)
            .WithStreamData(fileStream)
            .WithObjectSize(sizeBytes)
            .WithContentType(contentType)
            .WithHeaders(metaData);

        await _minio.PutObjectAsync(putArgs, cancellationToken).ConfigureAwait(false);

        return uniqueId;
    }

    private async Task EnsureBucketExistsAsync()
    {
        var beArgs = new BucketExistsArgs().WithBucket(AttachmentsBucket);
        if (!await _minio.BucketExistsAsync(beArgs).ConfigureAwait(false))
        {
            await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(AttachmentsBucket)).ConfigureAwait(false);
        }
    }

    private static class MetaKeys
    {
        public const string OriginalName = "original-filename";
        public const string CreatedBy = "created-by";
        public const string CreatedAt = "created-at";
    }
}