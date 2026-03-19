using System.Linq.Expressions;

namespace BindingChaos.SharedKernel.Specifications;

/// <summary>
/// Base class for specifications that encapsulate domain knowledge and business rules.
/// </summary>
/// <typeparam name="T">The type of entity this specification applies to.</typeparam>
public abstract class Specification<T>
{
    /// <summary>
    /// Gets a specification that matches all entities.
    /// </summary>
    public static readonly Specification<T> All = new IdentitySpecification<T>();

    /// <summary>
    /// Determines whether the specified entity satisfies this specification.
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity satisfies this specification; otherwise, false.</returns>
    public bool IsSatisfiedBy(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Func<T, bool> predicate = ToExpression().Compile();
        return predicate(entity);
    }

    /// <summary>
    /// Creates a new specification that combines this specification with another using AND logic.
    /// </summary>
    /// <param name="specification">The specification to combine with.</param>
    /// <returns>A new specification representing the AND combination.</returns>
    public Specification<T> And(Specification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        if (this == All)
        {
            return specification;
        }

        if (specification == All)
        {
            return this;
        }

        return new AndSpecification<T>(this, specification);
    }

    /// <summary>
    /// Creates a new specification that combines this specification with another using OR logic.
    /// </summary>
    /// <param name="specification">The specification to combine with.</param>
    /// <returns>A new specification representing the OR combination.</returns>
    public Specification<T> Or(Specification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        if (this == All || specification == All)
        {
            return All;
        }

        return new OrSpecification<T>(this, specification);
    }

    /// <summary>
    /// Creates a new specification that negates this specification.
    /// </summary>
    /// <returns>A new specification representing the negation of this specification.</returns>
    public Specification<T> Not() => new NotSpecification<T>(this);

    /// <summary>
    /// Converts this specification to an expression that can be used for database queries.
    /// </summary>
    /// <returns>An expression representing this specification.</returns>
    public abstract Expression<Func<T, bool>> ToExpression();
}

/// <summary>
/// Identity specification that matches all entities.
/// </summary>
/// <typeparam name="T">The type of entity this specification applies to.</typeparam>
internal sealed class IdentitySpecification<T> : Specification<T>
{
    /// <summary>
    /// Returns an expression that always evaluates to true.
    /// </summary>
    /// <returns>An expression that matches all entities.</returns>
    public override Expression<Func<T, bool>> ToExpression()
    {
        return x => true;
    }
}

/// <summary>
/// Specification that combines two specifications using AND logic.
/// </summary>
/// <typeparam name="T">The type of entity this specification applies to.</typeparam>
internal sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    /// <summary>
    /// Initializes a new instance of the AndSpecification class.
    /// </summary>
    /// <param name="left">The left specification.</param>
    /// <param name="right">The right specification.</param>
    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// Returns an expression that combines the left and right specifications using AND logic.
    /// </summary>
    /// <returns>An expression representing the AND combination.</returns>
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = leftExpression.Parameters.Single();
        var visitor = new ParameterReplacer(rightExpression.Parameters.Single(), parameter);
        var rightBody = visitor.Visit(rightExpression.Body);

        var andExpression = Expression.AndAlso(leftExpression.Body, rightBody);
        return Expression.Lambda<Func<T, bool>>(andExpression, parameter);
    }
}

/// <summary>
/// Specification that combines two specifications using OR logic.
/// </summary>
/// <typeparam name="T">The type of entity this specification applies to.</typeparam>
internal sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    /// <summary>
    /// Initializes a new instance of the OrSpecification class.
    /// </summary>
    /// <param name="left">The left specification.</param>
    /// <param name="right">The right specification.</param>
    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// Returns an expression that combines the left and right specifications using OR logic.
    /// </summary>
    /// <returns>An expression representing the OR combination.</returns>
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = leftExpression.Parameters.Single();
        var visitor = new ParameterReplacer(rightExpression.Parameters.Single(), parameter);
        var rightBody = visitor.Visit(rightExpression.Body);

        var orExpression = Expression.OrElse(leftExpression.Body, rightBody);
        return Expression.Lambda<Func<T, bool>>(orExpression, parameter);
    }
}

/// <summary>
/// Specification that negates another specification.
/// </summary>
/// <typeparam name="T">The type of entity this specification applies to.</typeparam>
internal sealed class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _specification;

    /// <summary>
    /// Initializes a new instance of the NotSpecification class.
    /// </summary>
    /// <param name="specification">The specification to negate.</param>
    public NotSpecification(Specification<T> specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    /// <summary>
    /// Returns an expression that negates the underlying specification.
    /// </summary>
    /// <returns>An expression representing the negation.</returns>
    public override Expression<Func<T, bool>> ToExpression()
    {
        var expression = _specification.ToExpression();
        var notExpression = Expression.Not(expression.Body);

        return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters.Single());
    }
}

/// <summary>
/// Expression visitor that replaces parameter expressions.
/// </summary>
internal sealed class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    /// <summary>
    /// Initializes a new instance of the ParameterReplacer class.
    /// </summary>
    /// <param name="oldParameter">The parameter to replace.</param>
    /// <param name="newParameter">The parameter to replace with.</param>
    public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter ?? throw new ArgumentNullException(nameof(oldParameter));
        _newParameter = newParameter ?? throw new ArgumentNullException(nameof(newParameter));
    }

    /// <summary>
    /// Visits a parameter expression and replaces it if it matches the old parameter.
    /// </summary>
    /// <param name="node">The parameter expression to visit.</param>
    /// <returns>The new parameter expression if it matches; otherwise, the original.</returns>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }
}