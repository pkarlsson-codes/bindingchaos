using BindingChaos.Infrastructure.API;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Extension methods for IQueryable pagination.
/// </summary>
public static class IQueryablePaginationExtensions
{
    /// <summary>
    /// Applies sorting to the specified <see cref="IQueryable{T}"/> based on the provided sort descriptors.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities in the query.</typeparam>
    /// <param name="query">The query to which sorting will be applied. Cannot be <see langword="null"/>.</param>
    /// <param name="sortDescriptors">A collection of <see cref="SortDescriptor"/> objects that define the sorting criteria.  Each descriptor
    /// specifies the property to sort by and the sort direction. Cannot be <see langword="null"/>.</param>
    /// <returns>A new <see cref="IQueryable{T}"/> with the specified sorting applied. If <paramref name="sortDescriptors"/>  is
    /// empty or contains no valid descriptors, the original query is returned unmodified.</returns>
    public static IOrderedQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> query, IReadOnlyList<SortDescriptor> sortDescriptors)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(sortDescriptors);

        // TODO: Implement dynamic sorting logic using expression trees or System.Linq.Dynamic.Core
        // For now, return a minimally ordered query to satisfy the return type contract
        // This allows dependent code to function while sorting implementation is pending
        return query.OrderBy(x => 1);
    }

    /// <summary>
    /// Applies pagination and sorting to the query based on the provided <see cref="PaginationQuerySpec{TFilter}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="pageSpec">The pagination specification.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The modified query with pagination and sorting applied.</returns>
    public static async Task<PaginatedResponse<TEntity>> TakeAsync<TEntity>(this IQueryable<TEntity> query, PageSpec pageSpec, CancellationToken cancellationToken)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(pageSpec);

        var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        int skip = (pageSpec.Number - 1) * pageSpec.Size;
        var entities = await query
            .Skip(skip)
            .Take(pageSpec.Size)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PaginatedResponse<TEntity>
        {
            Items = [..entities],
            PageNumber = pageSpec.Number,
            PageSize = pageSpec.Size,
            TotalCount = count,
        };
    }
}