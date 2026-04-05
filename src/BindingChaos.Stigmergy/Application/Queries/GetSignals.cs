using BindingChaos.CorePlatform.Contracts.Filters;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Specifications;
using BindingChaos.Stigmergy.Application.ReadModels;
using BindingChaos.Stigmergy.Application.Specifications;
using Marten;
using Marten.Pagination;

namespace BindingChaos.Stigmergy.Application.Queries;

/// <summary>
/// Query to retrieve signals with optional filtering and pagination.
/// </summary>
public sealed record GetSignals(PaginationQuerySpec<SignalsQueryFilter> QuerySpec);

/// <summary>Handles the <see cref="GetSignals"/> query.</summary>
public static class GetSignalsHandler
{
    /// <summary>Returns a paginated list of signals matching the query criteria.</summary>
    /// <param name="request">The query containing the filter specification.</param>
    /// <param name="querySession">Marten query session for read-model access.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated response of signals matching the query criteria.</returns>
    public static async Task<PaginatedResponse<SignalsListItemView>> Handle(
        GetSignals request,
        IQuerySession querySession,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var filter = request.QuerySpec.Filter;
        var cutoffDate = string.IsNullOrWhiteSpace(filter.TimeRange)
            ? (DateTimeOffset?)null
            : GetCutoffDate(filter.TimeRange);

        var amplificationRange = string.IsNullOrWhiteSpace(filter.AmplificationLevel)
            ? ((int min, int max)?)null
            : GetAmplificationRange(filter.AmplificationLevel);

        var query = querySession.Query<SignalsListItemView>().Matching(
            SignalsCapturedSinceSpecification.Optional(cutoffDate)
            .And(SignalsMatchingSearchTermSpecification.Optional(filter.SearchTerm))
            .And(SignalsWithAnyTagsSpecification.Optional(filter.Tags))
            .And(SignalsByAmplificationRangeSpecification.Optional(amplificationRange))
            .And(SignalsAmplifiedByParticipantSpecification.Optional(filter.AmplifiedByParticipantId)));

        query = query.Sort(request.QuerySpec.SortDescriptors, SignalsListItemView.SortMappings);

        var page = await query
            .ToPagedListAsync(request.QuerySpec.Page.Number, request.QuerySpec.Page.Size, cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<SignalsListItemView>
        {
            Items = [.. page],
            TotalCount = (int)page.TotalItemCount,
            PageSize = (int)page.PageSize,
            PageNumber = (int)page.PageNumber,
        };
    }

    private static DateTimeOffset GetCutoffDate(string timeRange)
    {
        return timeRange switch
        {
            "1h" => DateTimeOffset.UtcNow.AddHours(-1),
            "24h" => DateTimeOffset.UtcNow.AddHours(-24),
            "7d" => DateTimeOffset.UtcNow.AddDays(-7),
            "30d" => DateTimeOffset.UtcNow.AddDays(-30),
            _ => DateTimeOffset.UtcNow.AddDays(-30)
        };
    }

    private static (int min, int max) GetAmplificationRange(string amplificationLevel)
    {
        return amplificationLevel switch
        {
            "low" => (0, 2),
            "medium" => (3, 9),
            "high" => (10, int.MaxValue),
            _ => (0, int.MaxValue)
        };
    }
}
