using BindingChaos.SharedKernel.Domain;
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
}
