using System.Text;
using System.Text.Json;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace BindingChaos.CorePlatform.API.Infrastructure.Seeding;

/// <summary>
/// Seeds placeholder documents into MinIO for development use.
/// Each document is stored using its logical ID as the MinIO object key so that
/// document IDs in seed data can be resolved directly without a mapping step.
/// Seeding is idempotent: existing objects are skipped.
/// </summary>
internal static class DocumentSeeder
{
    private const string AttachmentsBucket = "attachments";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Uploads any missing seed documents to MinIO.
    /// </summary>
    /// <param name="minio">The MinIO client.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task SeedAsync(IMinioClient minio, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(minio);

        await EnsureBucketExistsAsync(minio, cancellationToken).ConfigureAwait(false);

        var data = await LoadSeedDataAsync(cancellationToken).ConfigureAwait(false);

        foreach (var doc in data.Documents)
        {
            if (await ObjectExistsAsync(minio, doc.Id, cancellationToken).ConfigureAwait(false))
            {
                continue;
            }

            var bytes = Encoding.UTF8.GetBytes(doc.Content);

#pragma warning disable CA2007
            await using var stream = new MemoryStream(bytes);
#pragma warning restore CA2007

            var putArgs = new PutObjectArgs()
                .WithBucket(AttachmentsBucket)
                .WithObject(doc.Id)
                .WithStreamData(stream)
                .WithObjectSize(bytes.Length)
                .WithContentType(doc.MimeType)
                .WithHeaders(new Dictionary<string, string>
                {
                    { "original-filename", doc.FileName },
                });

            await minio.PutObjectAsync(putArgs, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task<bool> ObjectExistsAsync(IMinioClient minio, string key, CancellationToken cancellationToken)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(AttachmentsBucket)
                .WithObject(key);

            await minio.StatObjectAsync(args, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
    }

    private static async Task EnsureBucketExistsAsync(IMinioClient minio, CancellationToken cancellationToken)
    {
        var beArgs = new BucketExistsArgs().WithBucket(AttachmentsBucket);
        if (!await minio.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false))
        {
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(AttachmentsBucket), cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task<DocumentSeedData> LoadSeedDataAsync(CancellationToken cancellationToken)
    {
        var stream = typeof(DocumentSeeder).Assembly
            .GetManifestResourceStream("BindingChaos.CorePlatform.API.Infrastructure.Seeding.seed-documents.json")
            ?? throw new InvalidOperationException("seed-documents.json embedded resource not found in CorePlatform.API assembly.");

#pragma warning disable CA2007
        await using (stream)
#pragma warning restore CA2007
        {
            return await JsonSerializer.DeserializeAsync<DocumentSeedData>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Failed to deserialize seed-documents.json.");
        }
    }

    private sealed class DocumentSeedData
    {
        public List<DocumentEntry> Documents { get; set; } = [];
    }

    private sealed class DocumentEntry
    {
        public string Id { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string MimeType { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}
