using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Unique identifier for a <see cref="UserGroup"/>.
/// </summary>
public sealed class UserGroupId : EntityId<UserGroupId>
{
    private const string Prefix = "usergroup";

    private UserGroupId(string value)
        : base(value, Prefix)
    {
    }
}
