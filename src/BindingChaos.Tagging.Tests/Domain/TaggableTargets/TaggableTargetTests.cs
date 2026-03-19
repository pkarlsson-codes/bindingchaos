using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Tagging.Domain.TaggableTargets;
using BindingChaos.Tagging.Domain.TaggableTargets.Events;
using BindingChaos.Tagging.Domain.Tags;
using FluentAssertions;

namespace BindingChaos.Tagging.Tests.Domain.TaggableTargets;

public class TaggableTargetTests
{
    private static readonly ParticipantId TestParticipant = ParticipantId.Generate();

    public class TheAssignTagsMethod
    {
        private static TaggableTarget CreateTarget()
        {
            var id = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            return new TaggableTarget(id);
        }

        [Fact]
        public void GivenOneNewTag_WhenTagsAssigned_ThenRaisesTagsAssignedEvent()
        {
            var sut = CreateTarget();
            sut.UncommittedEvents.MarkAsCommitted();
            var tagId = TagId.Generate();

            sut.AssignTags([tagId], TestParticipant);

            sut.UncommittedEvents.Should().ContainSingle(e => e is TagsAssigned);
        }

        [Fact]
        public void GivenTagAlreadyAssigned_WhenSameTagAssignedAgain_ThenRaisesNoEvent()
        {
            var sut = CreateTarget();
            var tagId = TagId.Generate();
            sut.AssignTags([tagId], TestParticipant);
            sut.UncommittedEvents.MarkAsCommitted();

            sut.AssignTags([tagId], TestParticipant);

            sut.UncommittedEvents.Should().BeEmpty();
        }

        [Fact]
        public void GivenEmptyArray_WhenTagsAssigned_ThenThrows()
        {
            var sut = CreateTarget();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.AssignTags([], TestParticipant);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenMixOfNewAndAlreadyAssignedTags_WhenTagsAssigned_ThenEventContainsOnlyNewTagIds()
        {
            var sut = CreateTarget();
            var existingTagId = TagId.Generate();
            sut.AssignTags([existingTagId], TestParticipant);
            sut.UncommittedEvents.MarkAsCommitted();

            var newTagId = TagId.Generate();
            sut.AssignTags([existingTagId, newTagId], TestParticipant);

            var assignedEvent = sut.UncommittedEvents.Should().ContainSingle(e => e is TagsAssigned)
                .Which.As<TagsAssigned>();
            assignedEvent.TagIds.Should().ContainSingle().Which.Should().Be(newTagId.Value);
        }
    }

    public class TheRemoveTagsMethod
    {
        private static TaggableTarget CreateTarget()
        {
            var id = TaggableTargetId.ForEntity("signal-01arz3ndektsv4rrffq69g5fav");
            return new TaggableTarget(id);
        }

        [Fact]
        public void GivenAssignedTag_WhenTagRemoved_ThenRaisesTagsRemovedEvent()
        {
            var sut = CreateTarget();
            var tagId = TagId.Generate();
            sut.AssignTags([tagId], TestParticipant);
            sut.UncommittedEvents.MarkAsCommitted();

            sut.RemoveTags([tagId], TestParticipant);

            sut.UncommittedEvents.Should().ContainSingle(e => e is TagsRemoved);
        }

        [Fact]
        public void GivenTagNotCurrentlyAssigned_WhenTagRemoved_ThenRaisesNoEvent()
        {
            var sut = CreateTarget();
            sut.UncommittedEvents.MarkAsCommitted();
            var tagId = TagId.Generate();

            sut.RemoveTags([tagId], TestParticipant);

            sut.UncommittedEvents.Should().BeEmpty();
        }

        [Fact]
        public void GivenEmptyArray_WhenTagsRemoved_ThenThrows()
        {
            var sut = CreateTarget();
            sut.UncommittedEvents.MarkAsCommitted();

            var act = () => sut.RemoveTags([], TestParticipant);

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }
}
