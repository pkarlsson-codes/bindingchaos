using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.CommunityDiscourse.Domain.Contributions;

/// <summary>
/// Repository interface for contribution persistence operations.
/// </summary>
public interface IContributionRepository : IRepository<Contribution, ContributionId>
{
}