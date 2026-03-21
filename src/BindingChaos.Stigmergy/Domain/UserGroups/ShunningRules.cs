using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.UserGroups;

/// <summary>
/// Value object describing the rules that govern shunning behaviour within a <see cref="UserGroup"/>.
/// This is a placeholder for future shunning policy properties.
/// </summary>
public sealed class ShunningRules : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShunningRules"/> class.
    /// </summary>
    public ShunningRules()
    {
    }

    /// <inheritdoc/>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}
