using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Tagging.Domain.Tags;
using BindingChaos.Tagging.Domain.Tags.Events;
using FluentAssertions;

namespace BindingChaos.Tagging.Tests.Domain.Tags;

public class TagTests
{
    private static readonly ParticipantId TestCreator = ParticipantId.Generate();

    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidInputs_WhenCreated_ThenRaisesTagCreatedEvent()
        {
            var sut = Tag.Create("Climate Change", TestCreator);

            sut.UncommittedEvents.Should().ContainSingle(e => e is TagCreated);
        }

        [Fact]
        public void GivenValidInputs_WhenCreated_ThenPreferredLabelIsSet()
        {
            var sut = Tag.Create("Climate Change", TestCreator);

            sut.PreferredLabel.Should().Be("Climate Change");
        }

        [Fact]
        public void GivenValidInputs_WhenCreated_ThenIsNotDeprecated()
        {
            var sut = Tag.Create("Climate Change", TestCreator);

            sut.IsDeprecated.Should().BeFalse();
        }

        [Fact]
        public void GivenEmptyLabel_WhenCreated_ThenThrows()
        {
            var act = () => Tag.Create(string.Empty, TestCreator);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenWhiteSpaceLabel_WhenCreated_ThenThrows()
        {
            var act = () => Tag.Create("   ", TestCreator);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenLabelExceeding50Characters_WhenCreated_ThenThrows()
        {
            var longLabel = new string('a', 51);

            var act = () => Tag.Create(longLabel, TestCreator);

            act.Should().Throw<BusinessRuleViolationException>();
        }

        [Fact]
        public void GivenLabelOfExactly50Characters_WhenCreated_ThenSucceeds()
        {
            var label = new string('a', 50);

            var sut = Tag.Create(label, TestCreator);

            sut.PreferredLabel.Should().Be(label);
        }
    }

    public class TheDeprecateMethod
    {
        [Fact]
        public void GivenActiveTag_WhenDeprecated_ThenRaisesTagDeprecatedEvent()
        {
            var sut = Tag.Create("Climate Change", TestCreator);
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Deprecate("Outdated term", TestCreator.Value);

            sut.UncommittedEvents.Should().ContainSingle(e => e is TagDeprecated);
        }

        [Fact]
        public void GivenActiveTag_WhenDeprecated_ThenIsDeprecatedIsTrue()
        {
            var sut = Tag.Create("Climate Change", TestCreator);
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Deprecate("Outdated term", TestCreator.Value);

            sut.IsDeprecated.Should().BeTrue();
        }

        [Fact]
        public void GivenAlreadyDeprecatedTag_WhenDeprecatedAgain_ThenRaisesNoEvent()
        {
            var sut = Tag.Create("Climate Change", TestCreator);
            sut.Deprecate("First deprecation", TestCreator.Value);
            sut.UncommittedEvents.MarkAsCommitted();

            sut.Deprecate("Second deprecation", TestCreator.Value);

            sut.UncommittedEvents.Should().BeEmpty();
        }
    }

    public class TheMergeIntoMethod
    {
        [Fact]
        public void GivenActiveTag_WhenMergedInto_ThenRaisesTagsMergedEvent()
        {
            var sut = Tag.Create("Climate Change", TestCreator);
            sut.UncommittedEvents.MarkAsCommitted();
            var target = TagId.Generate();

            sut.MergeInto(target, [], TestCreator);

            sut.UncommittedEvents.Should().ContainSingle(e => e is TagsMerged);
        }

        [Fact]
        public void GivenActiveTag_WhenMergedInto_ThenIsDeprecatedIsTrue()
        {
            var sut = Tag.Create("Climate Change", TestCreator);
            sut.UncommittedEvents.MarkAsCommitted();
            var target = TagId.Generate();

            sut.MergeInto(target, [], TestCreator);

            sut.IsDeprecated.Should().BeTrue();
        }

        [Fact]
        public void GivenActiveTag_WhenMergedInto_ThenMergedIntoIdIsSet()
        {
            var sut = Tag.Create("Climate Change", TestCreator);
            sut.UncommittedEvents.MarkAsCommitted();
            var target = TagId.Generate();

            sut.MergeInto(target, [], TestCreator);

            sut.MergedInto.Should().Be(target);
        }

        [Fact]
        public void GivenAlreadyDeprecatedTag_WhenMergedInto_ThenRaisesNoEvent()
        {
            var sut = Tag.Create("Climate Change", TestCreator);
            sut.Deprecate("Old", TestCreator.Value);
            sut.UncommittedEvents.MarkAsCommitted();
            var target = TagId.Generate();

            sut.MergeInto(target, [], TestCreator);

            sut.UncommittedEvents.Should().BeEmpty();
        }
    }
}
