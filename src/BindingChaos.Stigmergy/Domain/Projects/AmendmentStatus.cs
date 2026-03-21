using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// Represents the current status of a project amendment.
/// </summary>
public sealed class AmendmentStatus : Enumeration<AmendmentStatus>
{
    /// <summary>
    /// The amendment is active and in effect.
    /// </summary>
    public static readonly AmendmentStatus Active = new(1, nameof(Active));

    /// <summary>
    /// The amendment is being contested.
    /// </summary>
    public static readonly AmendmentStatus Contested = new(2, nameof(Contested));

    /// <summary>
    /// The amendment has been rejected.
    /// </summary>
    public static readonly AmendmentStatus Rejected = new(3, nameof(Rejected));

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
