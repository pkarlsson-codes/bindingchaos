namespace BindingChaos.Ideation.Application.ReadModels;

/// <summary>
/// View used to project minimal amendment info for ideas.
/// </summary>
public class AmendmentsListItemView
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
    /// The author identifier.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// The amendment title.
    /// </summary>
    public string AmendmentTitle { get; set; } = string.Empty;

    /// <summary>
    /// The amendment description.
    /// </summary>
    public string AmendmentDescription { get; set; } = string.Empty;

    /// <summary>
    /// The status of the amendment.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the collection of opponent identifiers.
    /// </summary>
    public List<string> OpponentIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of supporter IDs.
    /// </summary>
    public List<string> SupporterIds { get; set; } = [];

    /// <summary>
    /// When the amendment was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}
