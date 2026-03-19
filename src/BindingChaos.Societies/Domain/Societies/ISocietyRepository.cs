using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Societies.Domain.Societies;

/// <summary>
/// Repository interface for the <see cref="Society"/> aggregate.
/// </summary>
public interface ISocietyRepository : IRepository<Society, SocietyId>
{
}
