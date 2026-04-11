using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Domain.Projects;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>Query to retrieve inquiries for a given project.</summary>
/// <param name="ProjectId">The project whose inquiries to list.</param>
/// <param name="QuerySpec">Pagination and sort details.</param>
public sealed record GetProjectInquiries(ProjectId ProjectId, PaginationQuerySpec QuerySpec);

/// <summary>Handles the <see cref="GetProjectInquiries"/> query.</summary>
public static class GetProjectInquiriesHandler
{
    /// <summary>
    /// Returns a paginated list of inquiries for the specified project.
    /// </summary>
    /// <param name="request">The query.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of inquiry views.</returns>
    public static async Task<PaginatedResponse<ProjectInquiryView>> Handle(
        GetProjectInquiries request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = querySession.Query<ProjectInquiryView>()
            .Where(x => x.ProjectId == request.ProjectId.Value)
            .Sort(request.QuerySpec.SortDescriptors, ProjectInquiryView.SortMappings);

        var page = await query
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<ProjectInquiryView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }
}
