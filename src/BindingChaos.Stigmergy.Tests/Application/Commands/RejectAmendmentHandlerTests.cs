using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using FluentAssertions;
using Marten;
using Moq;

namespace BindingChaos.Stigmergy.Tests.Application.Commands;

public class RejectAmendmentHandlerTests
{
    private class TestBed
    {
        public Mock<IDocumentSession> Session { get; } = new();
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
        public async Task GivenProjectNotFound_WhenHandled_ThenThrowsInvalidOperation()
        {
            testBed.Session
                .Setup(s => s.LoadAsync<Project>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);

            var act = async () => await RejectAmendmentHandler.Handle(
                new RejectAmendment(ProjectId.Create("project-abc"), AmendmentId.Create("amendment-xyz")),
                testBed.Session.Object, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenAmendmentIsRejected()
        {
            var project = CreateProjectWithContestedAmendment(out var amendmentId);
            testBed.Session
                .Setup(s => s.LoadAsync<Project>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            await RejectAmendmentHandler.Handle(
                new RejectAmendment(project.Id, amendmentId),
                testBed.Session.Object, CancellationToken.None);

            project.Amendments.Single(a => a.Id == amendmentId).Status
                .Should().Be(AmendmentStatus.Rejected);
        }

        [Fact]
        public async Task GivenValidCommand_WhenHandled_ThenSavesChanges()
        {
            var project = CreateProjectWithContestedAmendment(out var amendmentId);
            testBed.Session
                .Setup(s => s.LoadAsync<Project>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            await RejectAmendmentHandler.Handle(
                new RejectAmendment(project.Id, amendmentId),
                testBed.Session.Object, CancellationToken.None);

            testBed.Session.Verify(s => s.Store(It.IsAny<Project>()), Times.Once);
            testBed.Session.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
