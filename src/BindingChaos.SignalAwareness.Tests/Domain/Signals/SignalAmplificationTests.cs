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
                ParticipantId.Generate());

            sut.IsActive.Should().BeTrue();
            sut.IsWithdrawn.Should().BeFalse();
        }

        [Fact]
        public void GivenNullAmplifierId_WhenCreated_ThenThrowsArgumentNullException()
        {
            var act = () => SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                null!);

            act.Should().Throw<ArgumentNullException>();
        }
    }

    public class TheAttenuateMethod
    {
        [Fact]
        public void GivenActiveAmplification_WhenAttenuated_ThenIsNoLongerActive()
        {
            var sut = SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                ParticipantId.Generate());

            sut.Attenuate();

            sut.IsActive.Should().BeFalse();
            sut.IsWithdrawn.Should().BeTrue();
        }

        [Fact]
        public void GivenAlreadyAttenuated_WhenAttenuatedAgain_ThenThrowsBusinessRuleViolationException()
        {
            var sut = SignalAmplification.Create(
                SignalAmplificationId.Generate(),
                ParticipantId.Generate());
            sut.Attenuate();

            var act = () => sut.Attenuate();

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }
}
