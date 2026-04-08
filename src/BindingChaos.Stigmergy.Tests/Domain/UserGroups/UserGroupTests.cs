using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;

namespace BindingChaos.Stigmergy.Tests.Domain.UserGroups;

public class UserGroupTests
{
    private static UserGroup CreateOpenGroup()
    {
        var charter = new Charter(
            new ContentionRules(0.5m, TimeSpan.FromDays(3)),
            new MembershipRules(JoinPolicy.Open, true, null, null, null),
            new ShunningRules(0.6m));
        return UserGroup.Form(
            ParticipantId.Generate(),
            CommonsId.Generate(),
            "Test Group",
            "Test Philosophy",
            charter);
    }

    public class TheJoinMethod
    {
        [Fact]
        public void GivenOpenGroup_WhenApplyingToJoin_ThenMemberIsAdded()
        {
            var group = CreateOpenGroup();
            var member = ParticipantId.Generate();

            group.ApplyToJoin(member);

            group.Members.Should().Contain(m => m.ParticipantId == member);
        }

        [Fact]
        public void GivenOpenGroup_WhenApplyingToJoin_ThenFounderRemainsMember()
        {
            var group = CreateOpenGroup();
            var founder = group.FounderId;
            var member = ParticipantId.Generate();

            group.ApplyToJoin(member);

            group.Members.Should().Contain(m => m.ParticipantId == founder);
        }
    }

    public class TheLeaveMethod
    {
        [Fact]
        public void GivenMember_WhenLeaving_ThenMemberIsRemoved()
        {
            var group = CreateOpenGroup();
            var member = ParticipantId.Generate();
            group.ApplyToJoin(member);

            group.Leave(member);

            group.Members.Should().NotContain(m => m.ParticipantId == member);
        }

        [Fact]
        public void GivenNonMember_WhenLeaving_ThenThrowsInvalidOperation()
        {
            var group = CreateOpenGroup();
            var nonMember = ParticipantId.Generate();

            var act = () => group.Leave(nonMember);

            act.Should().Throw<InvalidOperationException>();
        }
    }
}
