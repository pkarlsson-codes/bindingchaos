using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;

namespace BindingChaos.Stigmergy.Tests.Domain.Projects;

public class ProjectTests
{
    private static UserGroupId AUserGroupId() => UserGroupId.Generate();

    public class TheCreateMethod
    {
        [Fact]
        public void GivenValidArgs_WhenCreated_ThenProjectHasCorrectUserGroupId()
        {
            var userGroupId = AUserGroupId();
            var project = Project.Create(userGroupId, "Title", "Description");
            project.UserGroupId.Should().Be(userGroupId);
        }

        [Fact]
        public void GivenValidArgs_WhenCreated_ThenProjectHasId()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Description");
            project.Id.Should().NotBeNull();
        }

        [Fact]
        public void GivenValidArgs_WhenCreated_ThenProjectHasNoAmendments()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Description");
            project.Amendments.Should().BeEmpty();
        }
    }

    public class TheProposeAmendmentMethod
    {
        [Fact]
        public void GivenValidParticipant_WhenAmendmentProposed_ThenAmendmentAddedAsActive()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");
            var proposer = ParticipantId.Generate();

            project.ProposeAmendment(proposer);

            project.Amendments.Should().ContainSingle(a => a.Status == AmendmentStatus.Active);
        }

        [Fact]
        public void GivenValidParticipant_WhenAmendmentProposed_ThenReturnedIdMatchesStoredAmendment()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());

            project.Amendments.Should().ContainSingle(a => a.Id == amendmentId);
        }
    }

    public class TheContestAmendmentMethod
    {
        [Fact]
        public void GivenActiveAmendment_WhenContested_ThenAmendmentIsContested()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());
            var contester = ParticipantId.Generate();

            project.ContestAmendment(amendmentId, contester);

            project.Amendments.Single(a => a.Id == amendmentId).Status
                .Should().Be(AmendmentStatus.Contested);
        }

        [Fact]
        public void GivenNonExistentAmendment_WhenContested_ThenThrowsInvalidOperation()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");

            var act = () => project.ContestAmendment(AmendmentId.Generate(), ParticipantId.Generate());

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GivenAlreadyContestedAmendment_WhenContested_ThenThrowsBusinessRuleViolation()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());
            project.ContestAmendment(amendmentId, ParticipantId.Generate());

            var act = () => project.ContestAmendment(amendmentId, ParticipantId.Generate());

            act.Should().Throw<BusinessRuleViolationException>();
        }
    }

    public class TheRejectAmendmentMethod
    {
        [Fact]
        public void GivenContestedAmendment_WhenRejected_ThenAmendmentIsRejected()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());
            project.ContestAmendment(amendmentId, ParticipantId.Generate());

            project.RejectAmendment(amendmentId);

            project.Amendments.Single(a => a.Id == amendmentId).Status
                .Should().Be(AmendmentStatus.Rejected);
        }

        [Fact]
        public void GivenActiveAmendment_WhenRejected_ThenThrowsInvalidOperation()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());

            var act = () => project.RejectAmendment(amendmentId);

            act.Should().Throw<InvalidOperationException>();
        }
    }

    public class TheResolveContentionMethod
    {
        [Fact]
        public void GivenContestedAmendment_WhenResolved_ThenAmendmentIsActive()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());
            project.ContestAmendment(amendmentId, ParticipantId.Generate());

            project.ResolveContention(amendmentId);

            project.Amendments.Single(a => a.Id == amendmentId).Status
                .Should().Be(AmendmentStatus.Active);
        }

        [Fact]
        public void GivenActiveAmendment_WhenResolved_ThenThrowsInvalidOperation()
        {
            var project = Project.Create(AUserGroupId(), "Title", "Desc");
            var amendmentId = project.ProposeAmendment(ParticipantId.Generate());

            var act = () => project.ResolveContention(amendmentId);

            act.Should().Throw<InvalidOperationException>();
        }
    }
}
