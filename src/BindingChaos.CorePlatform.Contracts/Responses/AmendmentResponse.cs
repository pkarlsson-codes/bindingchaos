namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for a full amendment details.
/// </summary>
public sealed record AmendmentResponse
{
    /// <summary>
    /// Gets the unique identifier of the amendment.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ID of the target idea being amended.
    /// </summary>
    public string TargetIdeaId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the target version number of the idea this amendment applies to.
    /// </summary>
    public int TargetVersionNumber { get; init; }

    /// <summary>
    /// Gets the ID of the creator of this amendment.
    /// </summary>
    public string CreatorPseudonym { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the current user is the creator of the associated entity.
    /// </summary>
    public bool CreatedByCurrentUser { get; init; }

    /// <summary>
    /// Gets the current status of this amendment.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the proposed title for the amendment.
    /// </summary>
    public string ProposedTitle { get; init; } = string.Empty;

    /// <summary>
    /// Gets the proposed body for the amendment.
    /// </summary>
    public string ProposedBody { get; init; } = string.Empty;

    /// <summary>
    /// Gets the amendment title.
    /// </summary>
    public string AmendmentTitle { get; init; } = string.Empty;

    /// <summary>
    /// Gets the amendment description.
    /// </summary>
    public string AmendmentDescription { get; init; } = string.Empty;

    /// <summary>
    /// Gets when the amendment was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets when the amendment was accepted, if applicable.
    /// </summary>
    public DateTimeOffset? AcceptedAt { get; init; }

    /// <summary>
    /// Gets when the amendment was rejected, if applicable.
    /// </summary>
    public DateTimeOffset? RejectedAt { get; init; }

    /// <summary>
    /// Gets the number of supporters for this amendment.
    /// </summary>
    public int SupporterCount { get; init; }

    /// <summary>
    /// Gets the number of opponents for this amendment.
    /// </summary>
    public int OpponentCount { get; init; }

    /// <summary>
    /// Gets a value indicating whether this amendment is open for voting.
    /// </summary>
    public bool IsOpen { get; init; }

    /// <summary>
    /// Gets a value indicating whether this amendment has been resolved (accepted, rejected, or withdrawn).
    /// </summary>
    public bool IsResolved { get; init; }

    /// <summary>
    /// Gets a value indicating whether the current user has supported this amendment.
    /// </summary>
    public bool SupportedByCurrentUser { get; init; }

    /// <summary>
    /// Gets a value indicating whether the current user has opposed this amendment.
    /// </summary>
    public bool OpposedByCurrentUser { get; init; }
}