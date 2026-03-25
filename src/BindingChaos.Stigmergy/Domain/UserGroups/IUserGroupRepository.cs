using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Repository interface for UserGroup aggregate persistence operations.
/// </summary>
public interface IUserGroupRepository : IRepository<UserGroup, UserGroupId>
{
}
