namespace BindingChaos.Societies.Application.ReadModels;

/// <summary>
/// Lightweight read model for a social contract, used to look up the current contract for a society.
/// </summary>
public class SocialContractView
{
    /// <summary>
    /// Gets or sets the unique identifier of the social contract.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the society this contract belongs to.
    /// </summary>
    public string SocietyId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when this social contract was established.
    /// </summary>
    public DateTimeOffset EstablishedAt { get; set; }

    /// <summary>
    /// Gets or sets the inquiry lapse window in ticks (from the society's decision protocol).
    /// </summary>
    public long InquiryLapseWindowTicks { get; set; }
}
