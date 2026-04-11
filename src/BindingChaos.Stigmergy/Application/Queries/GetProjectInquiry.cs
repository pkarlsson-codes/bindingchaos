using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.ProjectInquiries;
using Marten;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve a single project inquiry by ID.</summary>
/// <param name="InquiryId">The inquiry identifier.</param>
public sealed record GetProjectInquiry(ProjectInquiryId InquiryId);

/// <summary>Handles the <see cref="GetProjectInquiry"/> query.</summary>
public static class GetProjectInquiryHandler
{
    /// <summary>
    /// Returns the inquiry view for the given ID, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The inquiry view, or <see langword="null"/> if not found.</returns>
    public static Task<ProjectInquiryView?> Handle(
        GetProjectInquiry request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return querySession.Query<ProjectInquiryView>()
            .Where(x => x.Id == request.InquiryId.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
