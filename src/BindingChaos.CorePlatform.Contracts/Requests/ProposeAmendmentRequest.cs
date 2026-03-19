namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for proposing an amendment to an existing idea.
/// </summary>
public sealed record ProposeAmendmentRequest
{
    /// <summary>
    /// Gets or sets the version number of the target idea this amendment applies to.
    /// </summary>
    public int TargetIdeaVersion { get; set; }

    /// <summary>
    /// Gets or sets the proposed title for the amendment.
    /// </summary>
    public string ProposedTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the proposed body/content for the amendment.
    /// </summary>
    public string ProposedBody { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the amendment.
    /// </summary>
    public string AmendmentTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the amendment.
    /// </summary>
    public string AmendmentDescription { get; set; } = string.Empty;
}