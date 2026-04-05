using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the UserGroup repository for event sourcing.
/// </summary>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger for the repository.</param>
internal class UserGroupRepository(
    IDocumentSession session,
    ILogger<MartenRepository<UserGroup, UserGroupId>> logger)
    : MartenRepository<UserGroup, UserGroupId>(session, logger), IUserGroupRepository
{
}
