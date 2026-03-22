using BindingChaos.Stigmergy.Domain.Projects;
using Marten;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Internal command issued by the amendment contention saga when the
/// rejection threshold is NOT met at the end of the resolution window.
/// </summary>
/// <param name="ProjectId">The identifier of the project.</param>
/// <param name="AmendmentId">The identifier of the amendment to restore.</param>
public sealed record RestoreAmendmentToActive(string ProjectId, string AmendmentId);

/// <summary>
/// Handles the <see cref="RestoreAmendmentToActive"/> command.
/// </summary>
public static class RestoreAmendmentToActiveHandler
{
    /// <summary>
    /// Resolves amendment contention, returning the amendment to Active status.
    /// </summary>
    /// <param name="command">The command containing restoration details.</param>
    /// <param name="session">The Marten document session.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        RestoreAmendmentToActive command,
        IDocumentSession session,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var project = await session.LoadAsync<Project>(ProjectId.Create(command.ProjectId), cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Project {command.ProjectId} not found.");

        project.ResolveContention(AmendmentId.Create(command.AmendmentId));
        session.Store(project);
        await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
