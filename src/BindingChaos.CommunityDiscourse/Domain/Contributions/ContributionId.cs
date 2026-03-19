using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.CommunityDiscourse.Domain.Contributions;

/// <summary>
/// Unique identifier for a <see cref="Contribution"/>.
/// </summary>
public sealed class ContributionId : EntityId<ContributionId>
{
    private const string Prefix = "contribution";

    private ContributionId(string value)
        : base(value, Prefix)
    {
    }
}