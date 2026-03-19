using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Unique identifier for a <see cref="SignalAttachment"/>.
/// </summary>
public sealed class SignalAttachmentId : EntityId<SignalAttachmentId>
{
    private const string Prefix = "attachment";

    private SignalAttachmentId(string value)
        : base(value, Prefix) { }
}