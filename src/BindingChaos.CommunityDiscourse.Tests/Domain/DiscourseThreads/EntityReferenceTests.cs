using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using FluentAssertions;

namespace BindingChaos.CommunityDiscourse.Tests.Domain.DiscourseThreads;

public class EntityReferenceTests
{
    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidEntityTypeAndId_WhenCreated_ThenEntityTypeIsNormalisedToLowerCase()
        {
            var sut = EntityReference.Create("Idea", "idea-abc123");

            sut.EntityType.Should().Be("idea");
        }

        [Fact]
        public void GivenValidEntityTypeAndId_WhenCreated_ThenEntityIdIsPreservedAsSupplied()
        {
            var entityId = "idea-abc123";

            var sut = EntityReference.Create("idea", entityId);

            sut.EntityId.Should().Be(entityId);
        }

        [Fact]
        public void GivenIdea_WhenCreated_ThenSucceeds()
        {
            var act = () => EntityReference.Create("idea", "idea-abc123");

            act.Should().NotThrow();
        }

        [Fact]
        public void GivenSignal_WhenCreated_ThenSucceeds()
        {
            var act = () => EntityReference.Create("signal", "signal-abc123");

            act.Should().NotThrow();
        }

        [Fact]
        public void GivenUnknownEntityType_WhenCreated_ThenThrowsArgumentException()
        {
            var act = () => EntityReference.Create("unknown-type", "some-id-123");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenNullEntityType_WhenCreated_ThenThrowsArgumentException()
        {
            var act = () => EntityReference.Create(null!, "idea-abc123");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenWhitespaceEntityType_WhenCreated_ThenThrowsArgumentException()
        {
            var act = () => EntityReference.Create("   ", "idea-abc123");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenNullEntityId_WhenCreated_ThenThrowsArgumentException()
        {
            var act = () => EntityReference.Create("idea", null!);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenWhitespaceEntityId_WhenCreated_ThenThrowsArgumentException()
        {
            var act = () => EntityReference.Create("idea", "   ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenEntityIdExceeding128Characters_WhenCreated_ThenThrowsArgumentException()
        {
            var longId = new string('a', 129);

            var act = () => EntityReference.Create("idea", longId);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GivenEntityIdOfExactly128Characters_WhenCreated_ThenSucceeds()
        {
            var maxLengthId = new string('a', 128);

            var act = () => EntityReference.Create("idea", maxLengthId);

            act.Should().NotThrow();
        }
    }

    public class TheEqualitySemantics
    {
        [Fact]
        public void GivenSameTypeAndId_WhenCompared_ThenAreEqual()
        {
            var first = EntityReference.Create("idea", "idea-abc123");
            var second = EntityReference.Create("idea", "idea-abc123");

            first.Should().Be(second);
        }

        [Fact]
        public void GivenDifferentType_WhenCompared_ThenAreNotEqual()
        {
            var first = EntityReference.Create("idea", "abc123");
            var second = EntityReference.Create("signal", "abc123");

            first.Should().NotBe(second);
        }

        [Fact]
        public void GivenUpperCaseAndLowerCaseType_WhenCompared_ThenAreEqual()
        {
            var first = EntityReference.Create("Idea", "idea-abc123");
            var second = EntityReference.Create("idea", "idea-abc123");

            first.Should().Be(second);
        }
    }

    public class TheTryCreateMethod
    {
        [Fact]
        public void GivenValidInputs_WhenTryCreated_ThenReturnsInstance()
        {
            var sut = EntityReference.TryCreate("idea", "idea-abc123");

            sut.Should().NotBeNull();
            sut!.EntityType.Should().Be("idea");
            sut.EntityId.Should().Be("idea-abc123");
        }

        [Fact]
        public void GivenNullEntityType_WhenTryCreated_ThenReturnsNull()
        {
            var sut = EntityReference.TryCreate(null, "idea-abc123");

            sut.Should().BeNull();
        }

        [Fact]
        public void GivenInvalidEntityType_WhenTryCreated_ThenReturnsNull()
        {
            var sut = EntityReference.TryCreate("unknown-type", "some-id-123");

            sut.Should().BeNull();
        }
    }
}
