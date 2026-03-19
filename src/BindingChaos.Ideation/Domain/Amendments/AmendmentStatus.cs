using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Amendments;

/// <summary>
/// Represents the current status of an amendment.
/// </summary>
public sealed class AmendmentStatus : Enumeration<AmendmentStatus>
{
    /// <summary>
    /// The amendment is open for deliberation.
    /// </summary>
    public static readonly AmendmentStatus Open = new(1, nameof(Open));

    /// <summary>
    /// The amendment has been approved and accepted.
    /// </summary>
    public static readonly AmendmentStatus Approved = new(2, nameof(Approved));

    /// <summary>
    /// The amendment has been merged into the target idea.
    /// </summary>
    public static readonly AmendmentStatus Merged = new(3, nameof(Merged));

    /// <summary>
    /// The amendment has been rejected.
    /// </summary>
    public static readonly AmendmentStatus Rejected = new(4, nameof(Rejected));

    /// <summary>
    /// The amendment has been withdrawn by its creator.
    /// </summary>
    public static readonly AmendmentStatus Withdrawn = new(5, nameof(Withdrawn));

    /// <summary>
    /// The amendment is outdated due to a new version of the target idea.
    /// </summary>
    public static readonly AmendmentStatus Outdated = new(6, nameof(Outdated));

    /// <summary>
    /// Initializes a new instance of the AmendmentStatus class.
    /// </summary>
    /// <param name="id">The numeric identifier for this status.</param>
    /// <param name="name">The name of this status.</param>
    private AmendmentStatus(int id, string name)
        : base(id, name)
    {
    }
}