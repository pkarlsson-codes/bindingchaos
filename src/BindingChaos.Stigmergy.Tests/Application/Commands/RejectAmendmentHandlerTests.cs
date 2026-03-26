using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class RejectAmendmentHandlerTests
{
    private class TestBed
    {
        public Mock<IProjectRepository> ProjectRepository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    }

    private static Project CreateProjectWithContestedAmendment(out AmendmentId amendmentId)
    {
        var project = Project.Create(UserGroupId.Generate(), "Title", "Desc");
        amendmentId = project.ProposeAmendment(ParticipantId.Generate());
        project.ContestAmendment(amendmentId, ParticipantId.Generate());
        return project;
    }

    public class TheHandleMethod
    {
        private readonly TestBed testBed = new();

        [Fact]
        public async Task GivenProjectNotFound_WhenHandled_ThenThrowsAggregateNotFoundException()
        {
            testBed.ProjectRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AggregateNotFoundException(typeof(Project), ProjectId.Create("project-abc")));

            var act = async () => await RejectAmendmentHandler.Handle(
                new RejectAmendment(ProjectId.Create("project-abc"), AmendmentId.Create("amendment-xyz")),
                testBed.ProjectRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            await act.Should().ThrowAsync<AggregateNotFoundException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenAmendmentIsRejected()
        {
            var project = CreateProjectWithContestedAmendment(out var amendmentId);
            testBed.ProjectRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            await RejectAmendmentHandler.Handle(
                new RejectAmendment(project.Id, amendmentId),
                testBed.ProjectRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            project.Amendments.Single(a => a.Id == amendmentId).Status
                .Should().Be(AmendmentStatus.Rejected);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenSavesChanges()
        {
            var project = CreateProjectWithContestedAmendment(out var amendmentId);
            testBed.ProjectRepository
                .Setup(r => r.GetByIdOrThrowAsync(It.IsAny<ProjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            await RejectAmendmentHandler.Handle(
                new RejectAmendment(project.Id, amendmentId),
                testBed.ProjectRepository.Object, testBed.UnitOfWork.Object, CancellationToken.None);

            testBed.ProjectRepository.Verify(r => r.Stage(It.IsAny<Project>()), Times.Once);
            testBed.UnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
