using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Societies.Domain.SocialContracts;

/// <summary>
/// Repository interface for the <see cref="SocialContract"/> aggregate.
/// </summary>
public interface ISocialContractRepository : IRepository<SocialContract, SocialContractId>
{
}
