using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.GoverningCommons.Events;
using FluentAssertions;

namespace BindingChaos.Stigmergy.Tests.Domain.GoverningCommons;

public class CommonsTests
{
    private static Commons ProposeCommons() =>
        Commons.Propose("Water Management", "Governing local water resources", ParticipantId.Generate());

    public class TheActivateMethod
    {
        [Fact]
        public void GivenProposedCommons_WhenActivated_StatusIsActive()
        {
            var commons = ProposeCommons();
            commons.UncommittedEvents.MarkAsCommitted();

            commons.Activate();

            commons.UncommittedEvents.Should().ContainSingle().Which.Should().BeOfType<CommonsActivated>();
        }

        [Fact]
        public void GivenAlreadyActiveCommons_WhenActivatedAgain_StatusRemainsActive()
        {
            var commons = ProposeCommons();
            commons.Activate();
            commons.UncommittedEvents.MarkAsCommitted();

            var action = () => commons.Activate();

            action.Should().Throw<BusinessRuleViolationException>()
                .WithMessage("Only proposed commons can be activated.");
        }
    }
}
