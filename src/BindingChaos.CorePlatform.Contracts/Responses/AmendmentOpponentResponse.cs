namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for an amendment opponent.
/// </summary>
public sealed record AmendmentOpponentResponse
{
    /// <summary>
    /// Gets the unique identifier of the opponent record.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the pseudonym of the opponent.
    /// </summary>
    public string Pseudonym { get; init; } = string.Empty;

    /// <summary>
    /// Gets the reason provided by the opponent for opposing the amendment.
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// Gets when the opposition was added.
    /// </summary>
    public string OpposedAt { get; init; } = string.Empty;
}
