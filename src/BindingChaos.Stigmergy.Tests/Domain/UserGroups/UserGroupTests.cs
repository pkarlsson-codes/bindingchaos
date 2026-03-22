using BindingChaos.SharedKernel.Domain;
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
        return UserGroup.Create(ParticipantId.Generate(), "Test Group", charter);
    }

    public class TheLeaveMethod
    {
        [Fact]
        public void GivenMemberWhoIsNotFounder_WhenLeaving_ThenMemberIsRemoved()
        {
            var group = CreateOpenGroup();
            var member = ParticipantId.Generate();
            group.ApplyToJoin(member);

            group.Leave(member);

            group.Members.Should().NotContain(m => m.ParticipantId == member);
        }

        [Fact]
        public void GivenMemberWhoIsNotFounder_WhenLeaving_ThenOtherMembersRemain()
        {
            var group = CreateOpenGroup();
            var founder = group.FounderId;
            var member = ParticipantId.Generate();
            group.ApplyToJoin(member);

            group.Leave(member);

            group.Members.Should().ContainSingle(m => m.ParticipantId == founder);
        }

        [Fact]
        public void GivenFounder_WhenLeaving_ThenFounderIsRemoved()
        {
            var group = CreateOpenGroup();
            var founder = group.FounderId;

            group.Leave(founder);

            group.Members.Should().NotContain(m => m.ParticipantId == founder);
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
