namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for an amendment supporter.
/// </summary>
public sealed record AmendmentSupporterResponse
{
    /// <summary>
    /// Gets the unique identifier of the supporter record.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the pseudonym of the supporter.
    /// </summary>
    public string Pseudonym { get; init; } = string.Empty;

    /// <summary>
    /// Gets the reason provided by the supporter for supporting the amendment.
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// Gets when the support was added.
    /// </summary>
    public string SupportedAt { get; init; } = string.Empty;
}
