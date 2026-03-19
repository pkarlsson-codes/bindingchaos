using BindingChaos.CommunityDiscourse.Domain.Contributions;
using BindingChaos.CommunityDiscourse.Domain.Contributions.Events;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.SharedKernel.Domain;
using FluentAssertions;

namespace BindingChaos.CommunityDiscourse.Tests.Domain.Contributions;

public class ContributionTests
{
    private static readonly DiscourseThreadId TestThreadId = DiscourseThreadId.Generate();
    private static readonly ParticipantId TestAuthorId = ParticipantId.Generate();
    private static readonly ContributionContent TestContent = ContributionContent.Create("This is a test contribution.");

    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidInputsWithNoParent_WhenCreated_ThenRaisesContributionAddedEvent()
        {
            var sut = Contribution.Create(TestThreadId, TestAuthorId, TestContent);

            sut.UncommittedEvents.Should().ContainSingle(e => e is ContributionAdded);
        }

        [Fact]
        public void GivenValidInputsWithNoParent_WhenCreated_ThenParentContributionIdIsNull()
        {
            var sut = Contribution.Create(TestThreadId, TestAuthorId, TestContent);

            sut.ParentContributionId.Should().BeNull();
        }

        [Fact]
        public void GivenValidInputsWithNoParent_WhenCreated_ThenThreadIdIsSet()
        {
            var sut = Contribution.Create(TestThreadId, TestAuthorId, TestContent);

            sut.ThreadId.Should().Be(TestThreadId);
        }

        [Fact]
        public void GivenValidInputsWithNoParent_WhenCreated_ThenAuthorIdIsSet()
        {
            var sut = Contribution.Create(TestThreadId, TestAuthorId, TestContent);

            sut.AuthorId.Should().Be(TestAuthorId);
        }

        [Fact]
        public void GivenValidInputsWithNoParent_WhenCreated_ThenStatusIsPublished()
        {
            var sut = Contribution.Create(TestThreadId, TestAuthorId, TestContent);

            sut.Status.Should().Be(ContributionStatus.Published);
        }

        [Fact]
        public void GivenValidContent_WhenCreated_ThenContentIsSet()
        {
            var content = ContributionContent.Create("This is my contribution.");

            var sut = Contribution.Create(TestThreadId, TestAuthorId, content);

            sut.Content.Value.Should().Be("This is my contribution.");
        }

        [Fact]
        public void GivenValidInputsWithParentId_WhenCreated_ThenRaisesContributionAddedEvent()
        {
            var parentId = ContributionId.Generate();

            var sut = Contribution.Create(TestThreadId, TestAuthorId, TestContent, parentId);

            sut.UncommittedEvents.Should().ContainSingle(e => e is ContributionAdded);
        }

        [Fact]
        public void GivenValidInputsWithParentId_WhenCreated_ThenParentContributionIdIsSet()
        {
            var parentId = ContributionId.Generate();

            var sut = Contribution.Create(TestThreadId, TestAuthorId, TestContent, parentId);

            sut.ParentContributionId.Should().Be(parentId);
        }
    }
}
