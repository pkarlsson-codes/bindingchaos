using BindingChaos.Stigmergy.Domain.UserGroups;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;
using BindingChaos.Stigmergy.Infrastructure.Projections;
using FluentAssertions;
using JasperFx.Events;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Infrastructure.Projections;

public class UserGroupListItemViewProjectionTests
{
    [Fact]
    public void GivenUserGroupFormed_WhenCreatingView_ThenMemberCountStartsAtZero()
    {
        var formedEvent = CreateFormedEvent("founder-1");

        var view = UserGroupListItemViewProjection.Create(formedEvent);

        view.MemberCount.Should().Be(0);
        view.MemberParticipantIds.Should().BeEmpty();
    }

    [Fact]
    public void GivenFounderJoinSequence_WhenApplied_ThenFounderCountIsOne()
    {
        var founderId = "founder-1";
        var formedEvent = CreateFormedEvent(founderId);
        var view = UserGroupListItemViewProjection.Create(formedEvent);

        UserGroupListItemViewProjection.Apply(
            view,
            new MemberJoined("group-1", "membership-1", founderId));

        view.MemberCount.Should().Be(1);
        view.MemberParticipantIds.Should().ContainSingle().Which.Should().Be(founderId);
    }

    [Fact]
    public void GivenDuplicateMemberJoined_WhenApplied_ThenMemberCountRemainsUnique()
    {
        var memberId = "member-1";
        var formedEvent = CreateFormedEvent("founder-1");
        var view = UserGroupListItemViewProjection.Create(formedEvent);

        UserGroupListItemViewProjection.Apply(
            view,
            new MemberJoined("group-1", "membership-1", memberId));
        UserGroupListItemViewProjection.Apply(
            view,
            new MemberJoined("group-1", "membership-2", memberId));

        view.MemberCount.Should().Be(1);
        view.MemberParticipantIds.Should().ContainSingle().Which.Should().Be(memberId);
    }

    [Fact]
    public void GivenDuplicateMemberIds_WhenMemberLeaves_ThenAllDuplicatesAreRemovedAndCountIsRecomputed()
    {
        var memberId = "member-1";
        var formedEvent = CreateFormedEvent("founder-1");
        var view = UserGroupListItemViewProjection.Create(formedEvent);

        view.MemberParticipantIds.Add(memberId);
        view.MemberParticipantIds.Add(memberId);
        view.MemberCount = view.MemberParticipantIds.Count;

        UserGroupListItemViewProjection.Apply(
            view,
            new MemberLeft("group-1", memberId));

        view.MemberParticipantIds.Should().NotContain(memberId);
        view.MemberCount.Should().Be(0);
    }

    private static IEvent<UserGroupFormed> CreateFormedEvent(string founderId)
    {
        var formed = new UserGroupFormed(
            "group-1",
            "commons-1",
            founderId,
            "Group",
            "Philosophy",
            new CharterRecord(
                new ContentionRulesRecord(0.5m, TimeSpan.FromDays(3)),
                new MembershipRulesRecord(JoinPolicy.Open.Value, true, null, null, null),
                new ShunningRulesRecord(0.6m)));

        var eventMock = new Mock<IEvent<UserGroupFormed>>();
        eventMock.SetupGet(e => e.Data).Returns(formed);
        eventMock.SetupGet(e => e.Timestamp).Returns(DateTimeOffset.UtcNow);

        return eventMock.Object;
    }
}
