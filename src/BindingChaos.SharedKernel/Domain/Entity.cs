using System.Diagnostics.CodeAnalysis;

namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Base class for all domain entities.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity's unique identifier.</typeparam>
public abstract class Entity<TIdentity>
    where TIdentity : EntityId
{
    /// <summary>
    /// Initializes a new instance of the Entity class.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Gets the unique identifier for this entity.
    /// </summary>
    [NotNull]
    public TIdentity Id { get; protected set; } = default!;

    /// <summary>
    /// Determines whether two entities are equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>True if the entities are equal; otherwise, false.</returns>
    public static bool operator ==(Entity<TIdentity>? left, Entity<TIdentity>? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two entities are not equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>True if the entities are not equal; otherwise, false.</returns>
    public static bool operator !=(Entity<TIdentity>? left, Entity<TIdentity>? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether the current entity is equal to another entity.
    /// </summary>
    /// <param name="obj">The entity to compare with the current entity.</param>
    /// <returns>True if the entities are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is not Entity<TIdentity> other)
        {
            return false;
        }

        if (IsTransient() || other.IsTransient())
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        return Id!.Equals(other.Id);
    }

    /// <summary>
    /// Gets the hash code for this entity.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Returns a string representation of this entity.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString()
    {
        return $"{GetType().Name}[{Id}]";
    }

    /// <summary>
    /// Determines whether this entity is in a transient state.
    /// </summary>
    /// <returns>True if the entity is transient; otherwise, false.</returns>
    protected virtual bool IsTransient()
    {
        return Id == null;
    }
}