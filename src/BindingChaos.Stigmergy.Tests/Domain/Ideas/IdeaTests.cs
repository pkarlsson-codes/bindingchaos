using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.Ideas;
using BindingChaos.Stigmergy.Domain.Ideas.Events;
using FluentAssertions;

namespace BindingChaos.Stigmergy.Tests.Domain.Ideas;

public class IdeaTests
{
    public class TheDraftMethod
    {
        [Fact]
        public void GivenValidInputs_WhenDrafted_ThenRaisesIdeaDraftedEvent()
        {
            var authorId = ParticipantId.Generate();

            var sut = Idea.Draft(authorId, "Better coordination", "A description");

            var e = sut.UncommittedEvents.Should().ContainSingle().Which.Should().BeOfType<IdeaDrafted>().Subject;
            e.AuthorId.Should().Be(authorId.Value);
            e.Title.Should().Be("Better coordination");
            e.Description.Should().Be("A description");
            sut.Id.Value.Should().StartWith("stigmergyidea");
        }

        [Fact]
        public void GivenNullAuthorId_WhenDrafted_ThenThrowsArgumentNullException()
        {
            var act = () => Idea.Draft(null!, "Title", "Description");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullOrWhiteSpaceTitle_WhenDrafted_ThenThrowsArgumentException()
        {
            var act = () => Idea.Draft(ParticipantId.Generate(), "  ", "Description");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenNullOrWhiteSpaceDescription_WhenDrafted_ThenThrowsArgumentException()
        {
            var act = () => Idea.Draft(ParticipantId.Generate(), "Title", "  ");

            act.Should().Throw<ArgumentException>();
        }
    }

    public class TheForkMethod
    {
        private static Idea CreateDraftIdea() =>
            Idea.Draft(ParticipantId.Generate(), "Original idea", "Original description");

        [Fact]
        public void GivenValidInputs_WhenForked_ThenRaisesIdeaForkedEvent()
        {
            var parent = CreateDraftIdea();
            var forkAuthorId = ParticipantId.Generate();

            var fork = parent.Fork(forkAuthorId, "Forked idea", "Forked description");

            var e = fork.UncommittedEvents.Should().ContainSingle().Which.Should().BeOfType<IdeaForked>().Subject;
            e.ParentIdeaId.Should().Be(parent.Id.Value);
            e.AuthorId.Should().Be(forkAuthorId.Value);
            e.Title.Should().Be("Forked idea");
            e.Description.Should().Be("Forked description");
        }

        [Fact]
        public void GivenNullAuthorId_WhenForked_ThenThrowsArgumentNullException()
        {
            var parent = CreateDraftIdea();

            var act = () => parent.Fork(null!, "Title", "Description");

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullOrWhiteSpaceTitle_WhenForked_ThenThrowsArgumentException()
        {
            var parent = CreateDraftIdea();

            var act = () => parent.Fork(ParticipantId.Generate(), "  ", "Description");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenNullOrWhiteSpaceDescription_WhenForked_ThenThrowsArgumentException()
        {
            var parent = CreateDraftIdea();

            var act = () => parent.Fork(ParticipantId.Generate(), "Title", "  ");

            act.Should().Throw<ArgumentException>();
        }
    }

    public class ThePublishMethod
    {
        private static (Idea idea, ParticipantId authorId) CreateDraftIdea()
        {
            var authorId = ParticipantId.Generate();
            var idea = Idea.Draft(authorId, "An idea", "A description");
            idea.UncommittedEvents.MarkAsCommitted();
            return (idea, authorId);
        }

        [Fact]
        public void GivenDraftAndAuthor_WhenPublished_ThenRaisesIdeaPublishedEvent()
        {
            var (sut, authorId) = CreateDraftIdea();

            sut.Publish(authorId);

            sut.UncommittedEvents.Should().ContainSingle(e => e is IdeaPublished);
        }

        [Fact]
        public void GivenNonAuthor_WhenPublished_ThenThrowsForbiddenException()
        {
            var (sut, _) = CreateDraftIdea();

            var act = () => sut.Publish(ParticipantId.Generate());

            act.Should().Throw<ForbiddenException>();
        }

        [Fact]
        public void GivenAlreadyPublishedIdea_WhenPublishedAgain_ThenThrowsBusinessRuleViolationException()
        {
            var (sut, authorId) = CreateDraftIdea();
            sut.Publish(authorId);
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Publish(authorId);

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheUpdateMethod
    {
        private static (Idea idea, ParticipantId authorId) CreateDraftIdea()
        {
            var authorId = ParticipantId.Generate();
            var idea = Idea.Draft(authorId, "An idea", "A description");
            idea.UncommittedEvents.MarkAsCommitted();
            return (idea, authorId);
        }

        [Fact]
        public void GivenDraftAndAuthor_WhenUpdated_ThenRaisesIdeaDraftUpdatedEvent()
        {
            var (sut, authorId) = CreateDraftIdea();

            sut.Update(authorId, "New title", "New description");

            sut.UncommittedEvents.Should().ContainSingle(e => e is IdeaDraftUpdated);
        }

        [Fact]
        public void GivenNonAuthor_WhenUpdated_ThenThrowsForbiddenException()
        {
            var (sut, _) = CreateDraftIdea();

            var act = () => sut.Update(ParticipantId.Generate(), "New title", "New description");

            act.Should().Throw<ForbiddenException>();
        }

        [Fact]
        public void GivenPublishedIdea_WhenUpdated_ThenThrowsBusinessRuleViolationException()
        {
            var (sut, authorId) = CreateDraftIdea();
            sut.Publish(authorId);
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Update(authorId, "New title", "New description");

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }
}
