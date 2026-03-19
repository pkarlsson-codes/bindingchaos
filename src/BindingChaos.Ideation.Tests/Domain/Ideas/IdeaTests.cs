using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.Ideation.Domain.Ideas.Events;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using FluentAssertions;

namespace BindingChaos.Ideation.Tests.Domain.Ideas;

public class IdeaTests
{
    private static readonly SocietyId TestSociety = SocietyId.Create("society-test");
    private static readonly ParticipantId TestCreator = ParticipantId.Generate();
    private static readonly string[] TestSignalRefs = ["signal-abc123"];
    private static readonly string[] NoTags = [];

    private static Idea CreateIdea() =>
        Idea.Author(TestSociety, TestCreator, "A title", "A body", TestSignalRefs, NoTags);

    public class TheAuthorMethod
    {
        [Fact]
        public void GivenValidInputs_WhenAuthored_ThenRaisesIdeaAuthoredEvent()
        {
            var sut = Idea.Author(TestSociety,TestCreator, "A title", "A body", TestSignalRefs, NoTags);

            sut.UncommittedEvents.Should().ContainSingle(e => e is IdeaAuthored);
        }

        [Fact]
        public void GivenValidInputs_WhenAuthored_ThenStatusIsPublished()
        {
            var sut = Idea.Author(TestSociety,TestCreator, "A title", "A body", TestSignalRefs, NoTags);

            sut.Status.Should().Be(IdeaStatus.Published);
        }

        [Fact]
        public void GivenValidInputs_WhenAuthored_ThenVersionNumberIsOne()
        {
            var sut = Idea.Author(TestSociety,TestCreator, "A title", "A body", TestSignalRefs, NoTags);

            sut.CurrentVersion.VersionNumber.Should().Be(1);
        }

        [Fact]
        public void GivenNoSignalReferences_WhenAuthored_ThenThrows()
        {
            var act = () => Idea.Author(TestSociety,TestCreator, "A title", "A body", [], NoTags);

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheCreateForkMethod
    {
        [Fact]
        public void GivenValidInputs_WhenForked_ThenRaisesIdeaForkedEvent()
        {
            var parentId = IdeaId.Generate();

            var sut = Idea.CreateFork(TestSociety,TestCreator, "Fork title", "Fork body", parentId, TestSignalRefs, NoTags);

            sut.UncommittedEvents.Should().ContainSingle(e => e is IdeaForked);
        }

        [Fact]
        public void GivenValidInputs_WhenForked_ThenParentIdeaIdIsSet()
        {
            var parentId = IdeaId.Generate();

            var sut = Idea.CreateFork(TestSociety,TestCreator, "Fork title", "Fork body", parentId, TestSignalRefs, NoTags);

            sut.ParentIdeaId.Should().Be(parentId);
        }
    }

    public class TheAddTagMethod
    {
        [Fact]
        public void GivenNewTag_WhenTagAdded_ThenRaisesTagAddedToIdeaEvent()
        {
            var sut = CreateIdea();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.AddTag(TestCreator, "tag-abc");

            sut.UncommittedEvents.Should().ContainSingle(e => e is TagAddedToIdea);
        }

        [Fact]
        public void GivenDuplicateTag_WhenTagAdded_ThenThrows()
        {
            var sut = Idea.Author(TestSociety,TestCreator, "A title", "A body", TestSignalRefs, ["tag-abc"]);
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AddTag(TestCreator, "tag-abc");

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheRemoveTagMethod
    {
        [Fact]
        public void GivenExistingTag_WhenTagRemoved_ThenRaisesTagRemovedFromIdeaEvent()
        {
            var sut = Idea.Author(TestSociety,TestCreator, "A title", "A body", TestSignalRefs, ["tag-abc"]);
            sut.UncommittedEvents.MarkAsCommitted();

            sut.RemoveTag(TestCreator, "tag-abc");

            sut.UncommittedEvents.Should().ContainSingle(e => e is TagRemovedFromIdea);
        }

        [Fact]
        public void GivenNonExistentTag_WhenTagRemoved_ThenThrows()
        {
            var sut = CreateIdea();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.RemoveTag(TestCreator, "tag-xyz");

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheAmendMethod
    {
        [Fact]
        public void GivenValidInputs_WhenAmended_ThenRaisesIdeaAmendedEvent()
        {
            var sut = CreateIdea();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Amend(AmendmentId.Generate(), "New title", "New body");

            sut.UncommittedEvents.Should().ContainSingle(e => e is IdeaAmended);
        }

        [Fact]
        public void GivenValidInputs_WhenAmended_ThenVersionNumberIncreases()
        {
            var sut = CreateIdea();
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Amend(AmendmentId.Generate(), "New title", "New body");

            sut.CurrentVersion.VersionNumber.Should().Be(2);
        }

        [Fact]
        public void GivenEmptyTitle_WhenAmended_ThenThrows()
        {
            var sut = CreateIdea();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Amend(AmendmentId.Generate(), "", "New body");

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenEmptyBody_WhenAmended_ThenThrows()
        {
            var sut = CreateIdea();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.Amend(AmendmentId.Generate(), "New title", "");

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheAddRequirementMethod
    {
        [Fact]
        public void GivenValidSpec_WhenRequirementAdded_ThenRaisesRequirementAddedEvent()
        {
            var sut = CreateIdea();
            sut.UncommittedEvents.MarkAsCommitted();
            var spec = new RequirementSpec("Workers", 10, "people", RequirementType.Competence);

            sut.AddRequirement(spec, TestCreator);

            sut.UncommittedEvents.Should().ContainSingle(e => e is RequirementAdded);
        }
    }
}
