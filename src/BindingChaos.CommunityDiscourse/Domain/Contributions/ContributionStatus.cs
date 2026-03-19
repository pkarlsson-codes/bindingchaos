using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.CommunityDiscourse.Domain.Contributions;

/// <summary>
/// Enumeration representing the possible statuses of a contribution.
/// Simplified to just the essential status - Published (which is the default and only status for now).
/// </summary>
public sealed class ContributionStatus : Enumeration<ContributionStatus>
{
    /// <summary>
    /// Contribution is published and visible.
    /// </summary>
    public static readonly ContributionStatus Published = new(1, nameof(Published));

    /// <summary>
    /// Initializes a new instance of the ContributionStatus class.
    /// </summary>
    /// <param name="id">The unique identifier for this status.</param>
    /// <param name="name">The name of this status.</param>
    private ContributionStatus(int id, string name)
        : base(id, name)
    {
    }
}