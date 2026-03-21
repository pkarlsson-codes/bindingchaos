using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// Represents the current status of a project amendment.
/// </summary>
public sealed class AmendmentStatus : Enumeration<AmendmentStatus>
{
    /// <summary>
    /// The amendment is open for deliberation.
    /// </summary>
    public static readonly AmendmentStatus Open = new(1, nameof(Open));

    /// <summary>
    /// The amendment has been accepted.
    /// </summary>
    public static readonly AmendmentStatus Accepted = new(2, nameof(Accepted));

    /// <summary>
    /// The amendment has been rejected.
    /// </summary>
    public static readonly AmendmentStatus Rejected = new(3, nameof(Rejected));

    /// <summary>
    /// The amendment has been withdrawn by its proposer.
    /// </summary>
    public static readonly AmendmentStatus Withdrawn = new(4, nameof(Withdrawn));

    /// <summary>
    /// Initializes a new instance of the <see cref="AmendmentStatus"/> class.
    /// </summary>
    /// <param name="id">The numeric identifier for this status.</param>
    /// <param name="name">The name of this status.</param>
    private AmendmentStatus(int id, string name)
        : base(id, name)
    {
    }
}
