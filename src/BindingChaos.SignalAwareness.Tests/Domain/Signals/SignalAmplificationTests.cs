using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SignalAwareness.Domain.Signals;
using FluentAssertions;

namespace BindingChaos.SignalAwareness.Tests.Domain.Signals;

public class SignalAmplificationTests
{
    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidParameters_WhenCreated_ThenIsActive()
        {
            var sut = SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                SignalId.Generate(),
                ParticipantId.Generate(),
                AmplificationReason.HighRelevance,
                "some commentary",
                DateTimeOffset.UtcNow);

            sut.IsActive.Should().BeTrue();
            sut.IsWithdrawn.Should().BeFalse();
        }

        [Fact]
        public void GivenNullSignalId_WhenCreated_ThenThrowsArgumentNullException()
        {
            var act = () => SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                null!,
                ParticipantId.Generate(),
                AmplificationReason.HighRelevance,
                null,
                DateTimeOffset.UtcNow);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullAmplifierId_WhenCreated_ThenThrowsArgumentNullException()
        {
            var act = () => SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                SignalId.Generate(),
                null!,
                AmplificationReason.HighRelevance,
                null,
                DateTimeOffset.UtcNow);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullCommentary_WhenCreated_ThenCommentaryIsNull()
        {
            var sut = SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                SignalId.Generate(),
                ParticipantId.Generate(),
                AmplificationReason.HighRelevance,
                null,
                DateTimeOffset.UtcNow);

            sut.Commentary.Should().BeNull();
        }
    }

    public class TheAttenuateMethod
    {
        [Fact]
        public void GivenActiveAmplification_WhenAttenuated_ThenIsNoLongerActive()
        {
            var sut = SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                SignalId.Generate(),
                ParticipantId.Generate(),
                AmplificationReason.HighRelevance,
                null,
                DateTimeOffset.UtcNow);

            sut.Attenuate(DateTimeOffset.UtcNow);

            sut.IsActive.Should().BeFalse();
            sut.IsWithdrawn.Should().BeTrue();
        }

        [Fact]
        public void GivenActiveAmplification_WhenAttenuated_ThenWithdrawnAtIsSet()
        {
            var withdrawnAt = DateTimeOffset.UtcNow.AddMinutes(5);
            var sut = SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                SignalId.Generate(),
                ParticipantId.Generate(),
                AmplificationReason.HighRelevance,
                null,
                DateTimeOffset.UtcNow);

            sut.Attenuate(withdrawnAt);

            sut.WithdrawnAt.Should().Be(withdrawnAt);
        }

        [Fact]
        public void GivenAlreadyAttenuated_WhenAttenuatedAgain_ThenThrowsBusinessRuleViolationException()
        {
            var sut = SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                SignalId.Generate(),
                ParticipantId.Generate(),
                AmplificationReason.HighRelevance,
                null,
                DateTimeOffset.UtcNow);
            sut.Attenuate(DateTimeOffset.UtcNow);

            var act = () => sut.Attenuate(DateTimeOffset.UtcNow.AddMinutes(1));

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }
}
