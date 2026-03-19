using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads.Events;
using FluentAssertions;

namespace BindingChaos.CommunityDiscourse.Tests.Domain.DiscourseThreads;

public class DiscourseThreadTests
{
    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidEntityReference_WhenCreated_ThenRaisesDiscourseThreadCreatedEvent()
        {
            var entityReference = EntityReference.Create("idea", "idea-abc123");

            var sut = DiscourseThread.Create(entityReference);

            sut.UncommittedEvents.Should().ContainSingle(e => e is DiscourseThreadCreated);
        }

        [Fact]
        public void GivenValidEntityReference_WhenCreated_ThenEntityReferenceIsSet()
        {
            var entityReference = EntityReference.Create("idea", "idea-abc123");

            var sut = DiscourseThread.Create(entityReference);

            sut.EntityReference.Should().Be(entityReference);
        }

    }
}
