using System.Linq.Expressions;
using BindingChaos.SharedKernel.Specifications;
using BindingChaos.Stigmergy.Application.ReadModels;

namespace BindingChaos.Stigmergy.Application.Specifications;

/// <summary>
/// Matches ideas whose title or description contain a search term.
/// </summary>
internal sealed class IdeasMatchingSearchTermSpecification : Specification<IdeasListItemView>
{
    private readonly string _searchTerm;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeasMatchingSearchTermSpecification"/> class.
    /// </summary>
    /// <param name="searchTerm">The search term to match.</param>
    public IdeasMatchingSearchTermSpecification(string searchTerm)
    {
        _searchTerm = !string.IsNullOrWhiteSpace(searchTerm)
            ? searchTerm
            : throw new ArgumentException("Search term cannot be null or whitespace.", nameof(searchTerm));
    }

    /// <summary>
    /// Creates an optional search-term specification.
    /// </summary>
    /// <param name="searchTerm">The optional search term.</param>
    /// <returns>A search-term specification when provided; otherwise, an identity specification.</returns>
    public static Specification<IdeasListItemView> Optional(string? searchTerm)
    {
        return string.IsNullOrWhiteSpace(searchTerm)
            ? Specification<IdeasListItemView>.All
            : new IdeasMatchingSearchTermSpecification(searchTerm);
    }

    /// <inheritdoc />
    public override Expression<Func<IdeasListItemView, bool>> ToExpression()
    {
        return idea =>
            idea.Title.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
            idea.Description.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Matches ideas authored by a specific participant.
/// </summary>
internal sealed class IdeasByAuthorSpecification : Specification<IdeasListItemView>
{
    private readonly string _authorId;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeasByAuthorSpecification"/> class.
    /// </summary>
    /// <param name="authorId">The author participant ID to match.</param>
    public IdeasByAuthorSpecification(string authorId)
    {
        _authorId = !string.IsNullOrWhiteSpace(authorId)
            ? authorId
            : throw new ArgumentException("Author ID cannot be null or whitespace.", nameof(authorId));
    }

    /// <summary>
    /// Creates an optional author specification.
    /// </summary>
    /// <param name="authorId">The optional author participant ID.</param>
    /// <returns>An author specification when provided; otherwise, an identity specification.</returns>
    public static Specification<IdeasListItemView> Optional(string? authorId)
    {
        return string.IsNullOrWhiteSpace(authorId)
            ? Specification<IdeasListItemView>.All
            : new IdeasByAuthorSpecification(authorId);
    }

    /// <inheritdoc />
    public override Expression<Func<IdeasListItemView, bool>> ToExpression()
    {
        return idea => idea.AuthorId == _authorId;
    }
}

/// <summary>
/// Matches ideas with a given status string.
/// </summary>
internal sealed class IdeasByStatusSpecification : Specification<IdeasListItemView>
{
    private readonly string _status;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdeasByStatusSpecification"/> class.
    /// </summary>
    /// <param name="status">The status to match.</param>
    public IdeasByStatusSpecification(string status)
    {
        _status = !string.IsNullOrWhiteSpace(status)
            ? status
            : throw new ArgumentException("Status cannot be null or whitespace.", nameof(status));
    }

    /// <summary>
    /// Creates an optional status specification.
    /// </summary>
    /// <param name="status">The optional status string.</param>
    /// <returns>A status specification when provided; otherwise, an identity specification.</returns>
    public static Specification<IdeasListItemView> Optional(string? status)
    {
        return string.IsNullOrWhiteSpace(status)
            ? Specification<IdeasListItemView>.All
            : new IdeasByStatusSpecification(status);
    }

    /// <inheritdoc />
    public override Expression<Func<IdeasListItemView, bool>> ToExpression()
    {
        return idea => idea.Status == _status;
    }
}
