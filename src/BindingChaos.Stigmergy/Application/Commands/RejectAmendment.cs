using BindingChaos.Stigmergy.Domain.Projects;
using Marten;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Internal command issued by the amendment contention saga when
/// the rejection threshold is met at the end of the resolution window.
/// </summary>
/// <param name="ProjectId">The identifier of the project.</param>
/// <param name="AmendmentId">The identifier of the amendment to reject.</param>
public sealed record RejectAmendment(ProjectId ProjectId, AmendmentId AmendmentId);

/// <summary>
/// Handles the <see cref="RejectAmendment"/> command.
/// </summary>
public static class RejectAmendmentHandler
{
    /// <summary>
    /// Rejects a Contested amendment, transitioning it to Rejected status permanently.
    /// </summary>
    /// <param name="command">The command containing rejection details.</param>
    /// <param name="session">The Marten document session.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        RejectAmendment command,
        IDocumentSession session,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var project = await session.LoadAsync<Project>(command.ProjectId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Project {command.ProjectId.Value} not found.");

        project.RejectAmendment(command.AmendmentId);
        session.Store(project);
        await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
