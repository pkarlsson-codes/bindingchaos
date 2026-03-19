using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Amendments;

/// <summary>
/// Unique identifier for an <see cref="Amendment"/>.
/// </summary>
public sealed class AmendmentId : EntityId<AmendmentId>
{
    private const string Prefix = "amendment";

    private AmendmentId(string value)
        : base(value, Prefix)
    {
    }
}