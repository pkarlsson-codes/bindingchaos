namespace BindingChaos.Ideation.Application.ReadModels;

/// <summary>
/// View used to project detailed amendment information for the amendment details page.
/// </summary>
public class AmendmentDetailView
{
    /// <summary>
    /// The amendment identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the idea this amendment belongs to.
    /// </summary>
    public string IdeaId { get; set; } = string.Empty;

    /// <summary>
    /// The target version number of the idea this amendment applies to.
    /// </summary>
    public int TargetVersionNumber { get; set; }

    /// <summary>
    /// The creator identifier.
    /// </summary>
    public string CreatorId { get; set; } = string.Empty;

    /// <summary>
    /// The amendment title.
    /// </summary>
    public string AmendmentTitle { get; set; } = string.Empty;

    /// <summary>
    /// The amendment description.
    /// </summary>
    public string AmendmentDescription { get; set; } = string.Empty;

    /// <summary>
    /// The proposed title for the amendment.
    /// </summary>
    public string ProposedTitle { get; set; } = string.Empty;

    /// <summary>
    /// The proposed body for the amendment.
    /// </summary>
    public string ProposedBody { get; set; } = string.Empty;

    /// <summary>
    /// The status of the amendment.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// When the amendment was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the amendment was accepted, if applicable.
    /// </summary>
    public DateTimeOffset? AcceptedAt { get; set; }

    /// <summary>
    /// When the amendment was rejected, if applicable.
    /// </summary>
    public DateTimeOffset? RejectedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of supporter IDs.
    /// </summary>
    public List<string> SupporterIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of opponent identifiers.
    /// </summary>
    public List<string> OpponentIds { get; set; } = [];
}
