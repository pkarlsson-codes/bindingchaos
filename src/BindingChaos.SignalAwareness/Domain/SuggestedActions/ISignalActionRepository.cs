using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.SignalAwareness.Domain.SuggestedActions;

/// <summary>
/// A repository for <see cref="SuggestedAction"/>.
/// </summary>
public interface ISignalActionRepository : IRepository<SuggestedAction, SuggestedActionId> { }