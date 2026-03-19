namespace BindingChaos.DocumentProcessing.DTOs;

/// <summary>
/// An S3 data record.
/// </summary>
/// <param name="Object">An S3 object.</param>
/// <param name="Bucket">An S3 bucket.</param>
#pragma warning disable CA1720 // Identifier contains type name
public record S3Data(S3Object Object, S3Bucket Bucket);
#pragma warning restore CA1720 // Identifier contains type name
