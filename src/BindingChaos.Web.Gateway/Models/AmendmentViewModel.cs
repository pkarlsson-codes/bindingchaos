namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for idea amendments, tailored for frontend display.
/// </summary>
public sealed class AmendmentViewModel
{
    /// <summary>
    /// The unique identifier of the amendment.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The title of the amendment.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A short description of the amendment for display in lists.
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// The proposed title for the idea.
    /// </summary>
    public string ProposedTitle { get; set; } = string.Empty;

    /// <summary>
    /// The proposed body/content for the idea.
    /// </summary>
    public string ProposedBody { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the proposal was made by the current user.
    /// </summary>
    public bool ProposedByCurrentUser { get; set; }

    /// <summary>
    /// The pseudonym of the proposer.
    /// </summary>
    public string ProposerPseudonym { get; set; } = string.Empty;

    /// <summary>
    /// The status of the amendment: open, approved, rejected, withdrawn, outdated.
    /// </summary>
    public string Status { get; set; } = "open";

    /// <summary>
    /// The version number of the target idea this amendment applies to.
    /// </summary>
    public int TargetVersionNumber { get; set; }

    /// <summary>
    /// The creation timestamp in ISO 8601 format.
    /// </summary>
    public string PropsedAt { get; set; } = string.Empty;

    /// <summary>
    /// The total number of supporters.
    /// </summary>
    public int SupporterCount { get; set; }

    /// <summary>
    /// The total number of opponents.
    /// </summary>
    public int OpponentCount { get; set; }

    /// <summary>
    /// The acceptance timestamp in ISO 8601 format (if approved).
    /// </summary>
    public string? AcceptedOn { get; set; }

    /// <summary>
    /// The rejection timestamp in ISO 8601 format (if rejected).
    /// </summary>
    public string? RejectedOn { get; set; }

    /// <summary>
    /// Whether the current user has supported this amendment.
    /// </summary>
    public bool SupportedByCurrentUser { get; set; }

    /// <summary>
    /// Whether the current user has opposed this amendment.
    /// </summary>
    public bool OpposedByCurrentUser { get; set; }
}
