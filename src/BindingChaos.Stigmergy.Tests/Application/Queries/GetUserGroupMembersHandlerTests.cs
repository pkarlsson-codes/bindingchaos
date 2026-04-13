using BindingChaos.IdentityProfile.Application.Services;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.Queries;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Marten;
using Moq;

// NOTE: Scenarios that require a paginated query against UserGroupMembersView
// (public member list — returns results; private list + authenticated member — returns results)
// use IQuerySession.Query<T>() which returns IMartenQueryable<T>. Marten's async LINQ operators
// cannot be backed by an in-memory IQueryable<T> in Moq without implementing the full Marten
// query provider. Those cases are deferred to BindingChaos.CorePlatform.API.IntegrationTests.

namespace BindingChaos.Stigmergy.Tests.Application.Queries;

public class GetUserGroupMembersHandlerTests
{
    private class TestBed
    {
        public Mock<IQuerySession> QuerySession { get; } = new();
        public Mock<IUserGroupRepository> UserGroupRepository { get; } = new();
        public Mock<IPseudonymLookupService> PseudonymLookupService { get; } = new();

        public Task<PaginatedResponse<UserGroupMemberView>?> InvokeAsync(
            GetUserGroupMembers query) =>
            GetUserGroupMembersHandler.Handle(
                query,
                QuerySession.Object,
                UserGroupRepository.Object,
                PseudonymLookupService.Object,
                CancellationToken.None);
    }

    private static UserGroup BuildUserGroup(bool memberListPublic)
    {
        var charter = new Charter(
            new ContentionRules(0.4m, TimeSpan.FromDays(3)),
            new MembershipRules(JoinPolicy.Open, memberListPublic),
            new ShunningRules(0.6m));
        return UserGroup.Form(ParticipantId.Generate(), CommonsId.Generate(), "Test Group", "Philosophy", charter);
    }

    private static PaginationQuerySpec DefaultQuerySpec() => new() { Page = new PageSpec { Number = 1, Size = 20 } };

    public class GivenGroupNotFound
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task WhenHandled_ThenReturnsNull()
        {
            var userGroupId = UserGroupId.Generate();
            testBed.UserGroupRepository
                .Setup(r => r.GetByIdAsync(userGroupId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserGroup?)null);

            var result = await testBed.InvokeAsync(
                new GetUserGroupMembers(userGroupId, null, DefaultQuerySpec()));

            result.Should().BeNull();
        }
    }

    public class GivenPrivateMemberList
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenNonMemberCaller_WhenHandled_ThenThrowsUnauthorizedAccessException()
        {
            var userGroupId = UserGroupId.Generate();
            var callerId = ParticipantId.Generate();
            var userGroup = BuildUserGroup(memberListPublic: false);

            testBed.UserGroupRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);
            testBed.QuerySession
                .Setup(s => s.LoadAsync<UserGroupListItemView>(userGroupId.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserGroupListItemView
                {
                    Id = userGroupId.Value,
                    CommonsId = CommonsId.Generate().Value,
                    Name = "Test Group",
                    Philosophy = "Philosophy",
                    FounderId = ParticipantId.Generate().Value,
                    FormedAt = DateTimeOffset.UtcNow,
                    MemberCount = 1,
                    JoinPolicy = "Open",
                    MemberParticipantIds = [],
                });

            var act = async () => await testBed.InvokeAsync(
                new GetUserGroupMembers(userGroupId, callerId, DefaultQuerySpec()));

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task GivenAnonymousCaller_WhenHandled_ThenThrowsUnauthorizedAccessException()
        {
            var userGroupId = UserGroupId.Generate();
            var userGroup = BuildUserGroup(memberListPublic: false);

            testBed.UserGroupRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<UserGroupId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userGroup);
            testBed.QuerySession
                .Setup(s => s.LoadAsync<UserGroupListItemView>(userGroupId.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserGroupListItemView
                {
                    Id = userGroupId.Value,
                    CommonsId = CommonsId.Generate().Value,
                    Name = "Test Group",
                    Philosophy = "Philosophy",
                    FounderId = ParticipantId.Generate().Value,
                    FormedAt = DateTimeOffset.UtcNow,
                    MemberCount = 1,
                    JoinPolicy = "Open",
                    MemberParticipantIds = [],
                });

            var act = async () => await testBed.InvokeAsync(
                new GetUserGroupMembers(userGroupId, null, DefaultQuerySpec()));

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
